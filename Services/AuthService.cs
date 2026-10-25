using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>
/// Comptes réels, enregistrés LOCALEMENT sur le téléphone (aucun serveur) : mot de passe protégé par PBKDF2,
/// refus des doublons, erreurs explicites, session mémorisable. Les données (produits, ventes, kiosque)
/// sont séparées par compte : un nouveau compte démarre toujours à zéro.
/// </summary>
public class AuthService : IAuthService
{
    public const int LongueurMinimale = 6;

    private static readonly SemaphoreSlim Verrou = new(1, 1);

    private readonly ICompteRepository _comptes;
    private readonly ISession _session;
    private readonly IStockageSession _stockage;
    private readonly IKiosqueRepository _kiosques;

    public AuthService(ICompteRepository comptes, ISession session, IStockageSession stockage, IKiosqueRepository kiosques)
    {
        _comptes = comptes;
        _session = session;
        _stockage = stockage;
        _kiosques = kiosques;
    }

    public Utilisateur UtilisateurCourant => _session.Courant;

    public async Task<Utilisateur> ConnexionAsync(string identifiant, string motDePasse, bool memoriser)
    {
        if (string.IsNullOrWhiteSpace(identifiant) || string.IsNullOrEmpty(motDePasse))
            throw new AuthentificationException("Saisissez votre e-mail ou téléphone et votre mot de passe.");

        var cle = ValidationHelper.NormaliserIdentifiant(identifiant);
        var compte = (await _comptes.ListerAsync()).FirstOrDefault(c => Correspond(c, cle));
        if (compte == null)
            throw new AuthentificationException("Aucun compte ne correspond à cet identifiant. Créez un compte pour commencer.");

        if (!MotDePasseHasher.Verifier(motDePasse, compte.Sel, compte.Hash, compte.Iterations))
            throw new AuthentificationException("Mot de passe incorrect.");

        var utilisateur = VersUtilisateur(compte);
        _session.Ouvrir(utilisateur);
        if (memoriser) _stockage.MemoriserCompte(compte.Id); else _stockage.Oublier();
        return utilisateur;
    }

    public async Task<Utilisateur> InscriptionAsync(string identifiant, string nomKiosque, string telephone, string motDePasse)
    {
        if (string.IsNullOrWhiteSpace(nomKiosque) || nomKiosque.Trim().Length < 2)
            throw new AuthentificationException("Le nom du kiosque est obligatoire (2 caractères minimum).");
        if (!ValidationHelper.ContactValide(identifiant))
            throw new AuthentificationException("E-mail ou téléphone invalide (mobile à 9 chiffres commençant par 6).");
        if (!ValidationHelper.TelephoneValide(telephone))
            throw new AuthentificationException("Numéro de téléphone invalide : 9 chiffres commençant par 6 (ex. 6 99 12 34 56).");
        if (string.IsNullOrEmpty(motDePasse) || motDePasse.Length < LongueurMinimale)
            throw new AuthentificationException($"Le mot de passe doit contenir au moins {LongueurMinimale} caractères.");

        var cle = ValidationHelper.NormaliserIdentifiant(identifiant);
        var tel = ValidationHelper.ChiffresTelephone(telephone);

        await Verrou.WaitAsync();
        try
        {
            var existants = await _comptes.ListerAsync();
            if (existants.Any(c => Correspond(c, cle) || Correspond(c, tel)))
                throw new AuthentificationException("Un compte existe déjà avec cet e-mail ou ce téléphone. Connectez-vous.");

            var (sel, hash, iterations) = MotDePasseHasher.Hasher(motDePasse);
            var compte = await _comptes.AjouterAsync(new CompteLocal
            {
                Identifiant = cle, Telephone = tel, NomKiosque = nomKiosque.Trim(),
                Sel = sel, Hash = hash, Iterations = iterations, DateCreation = DateTime.Now
            });

            var utilisateur = VersUtilisateur(compte);
            _session.Ouvrir(utilisateur); // les données de ce compte vivent dans leur propre base, vide au départ
            _stockage.Oublier();

            try
            {
                await _kiosques.EnregistrerAsync(new Kiosque { Id = 1, Nom = compte.NomKiosque, Telephone = tel });
            }
            catch
            {
                _session.Fermer();
                throw;
            }
            return utilisateur;
        }
        finally
        {
            Verrou.Release();
        }
    }

    public async Task<Utilisateur> RestaurerSessionAsync()
    {
        var id = _stockage.LireCompteMemorise();
        if (id == null)
            return null;

        var compte = (await _comptes.ListerAsync()).FirstOrDefault(c => c.Id == id.Value);
        if (compte == null)
        {
            _stockage.Oublier();
            return null;
        }

        var utilisateur = VersUtilisateur(compte);
        _session.Ouvrir(utilisateur);
        return utilisateur;
    }

    public Task DeconnexionAsync()
    {
        _session.Fermer();
        _stockage.Oublier();
        return Task.CompletedTask;
    }

    private static bool Correspond(CompteLocal compte, string cle)
        => !string.IsNullOrEmpty(cle) && (compte.Identifiant == cle || compte.Telephone == cle);

    private static Utilisateur VersUtilisateur(CompteLocal c) => new()
    {
        Id = c.Id,
        NomKiosque = c.NomKiosque,
        Email = c.Identifiant.Contains('@') ? c.Identifiant : string.Empty,
        Telephone = c.Telephone
    };
}
