using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>
/// Comptes réels, enregistrés LOCALEMENT sur le téléphone (aucun serveur) : mot de passe protégé par PBKDF2,
/// refus des doublons, erreurs explicites, session mémorisable, mot de passe oublié par code de récupération.
/// Les données (produits, ventes, kiosque) sont séparées par compte : un compte neuf démarre toujours à zéro.
/// </summary>
public class AuthService : IAuthService
{
    public const int LongueurMinimale = 6;

    private static readonly SemaphoreSlim Verrou = new(1, 1);

    private readonly ICompteRepository _comptes;
    private readonly ISession _session;
    private readonly IStockageSession _stockage;
    private readonly IKiosqueRepository _kiosques;
    private readonly LimiteurTentatives _limiteur;

    public AuthService(ICompteRepository comptes, ISession session, IStockageSession stockage, IKiosqueRepository kiosques,
                       LimiteurTentatives limiteur = null)
    {
        _comptes = comptes;
        _session = session;
        _stockage = stockage;
        _kiosques = kiosques;
        _limiteur = limiteur ?? new LimiteurTentatives();
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

        VerifierPasBloque($"connexion-{compte.Id}");

        if (!MotDePasseHasher.Verifier(motDePasse, compte.Sel, compte.Hash, compte.Iterations))
        {
            _limiteur.EnregistrerEchec($"connexion-{compte.Id}");
            throw new AuthentificationException("Mot de passe incorrect.");
        }

        _limiteur.Reinitialiser($"connexion-{compte.Id}");
        var utilisateur = VersUtilisateur(compte);
        _session.Ouvrir(utilisateur);
        if (memoriser) _stockage.MemoriserCompte(compte.Id); else _stockage.Oublier();
        return utilisateur;
    }

    public async Task<ResultatInscription> InscriptionAsync(string identifiant, string nomKiosque, string telephone, string motDePasse)
    {
        if (string.IsNullOrWhiteSpace(nomKiosque) || nomKiosque.Trim().Length < 2)
            throw new AuthentificationException("Le nom du kiosque est obligatoire (2 caractères minimum).");
        if (!ValidationHelper.ContactValide(identifiant))
            throw new AuthentificationException("E-mail ou téléphone invalide (mobile à 9 chiffres commençant par 6).");
        if (!ValidationHelper.TelephoneValide(telephone))
            throw new AuthentificationException("Numéro de téléphone invalide : 9 chiffres commençant par 6 (ex. 6 99 12 34 56).");
        VerifierMotDePasse(motDePasse);

        var cle = ValidationHelper.NormaliserIdentifiant(identifiant);
        var tel = ValidationHelper.ChiffresTelephone(telephone);

        await Verrou.WaitAsync();
        try
        {
            var existants = await _comptes.ListerAsync();
            if (existants.Any(c => Correspond(c, cle) || Correspond(c, tel)))
                throw new AuthentificationException("Un compte existe déjà avec cet e-mail ou ce téléphone. Connectez-vous.");

            var (sel, hash, iterations) = MotDePasseHasher.Hasher(motDePasse);
            var code = CodeRecuperation.Generer();
            var (selCode, hashCode, iterationsCode) = MotDePasseHasher.Hasher(CodeRecuperation.Normaliser(code));

            var compte = await _comptes.AjouterAsync(new CompteLocal
            {
                Identifiant = cle, Telephone = tel, NomKiosque = nomKiosque.Trim(),
                Sel = sel, Hash = hash, Iterations = iterations,
                SelRecuperation = selCode, HashRecuperation = hashCode, IterationsRecuperation = iterationsCode,
                DateCreation = DateTime.Now
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
            return new ResultatInscription { Utilisateur = utilisateur, CodeRecuperation = code };
        }
        finally
        {
            Verrou.Release();
        }
    }

    public async Task<string> ReinitialiserMotDePasseAsync(string identifiant, string codeRecuperation, string nouveauMotDePasse)
    {
        if (string.IsNullOrWhiteSpace(identifiant))
            throw new AuthentificationException("Saisissez votre e-mail ou téléphone.");
        if (string.IsNullOrWhiteSpace(CodeRecuperation.Normaliser(codeRecuperation)))
            throw new AuthentificationException("Saisissez votre code de récupération.");
        VerifierMotDePasse(nouveauMotDePasse);

        var cle = ValidationHelper.NormaliserIdentifiant(identifiant);

        await Verrou.WaitAsync();
        try
        {
            var compte = (await _comptes.ListerAsync()).FirstOrDefault(c => Correspond(c, cle))
                         ?? throw new AuthentificationException("Aucun compte ne correspond à cet identifiant.");

            if (string.IsNullOrEmpty(compte.HashRecuperation))
                throw new AuthentificationException("Ce compte n'a pas de code de récupération : son mot de passe ne peut pas être réinitialisé.");

            VerifierPasBloque($"recuperation-{compte.Id}");

            if (!MotDePasseHasher.Verifier(CodeRecuperation.Normaliser(codeRecuperation), compte.SelRecuperation, compte.HashRecuperation, compte.IterationsRecuperation))
            {
                _limiteur.EnregistrerEchec($"recuperation-{compte.Id}");
                throw new AuthentificationException("Code de récupération incorrect.");
            }
            _limiteur.Reinitialiser($"recuperation-{compte.Id}");
            _limiteur.Reinitialiser($"connexion-{compte.Id}");

            // Nouveau mot de passe + NOUVEAU code (un code ne sert qu'une fois)
            var (sel, hash, iterations) = MotDePasseHasher.Hasher(nouveauMotDePasse);
            var nouveauCode = CodeRecuperation.Generer();
            var (selCode, hashCode, iterationsCode) = MotDePasseHasher.Hasher(CodeRecuperation.Normaliser(nouveauCode));
            compte.Sel = sel; compte.Hash = hash; compte.Iterations = iterations;
            compte.SelRecuperation = selCode; compte.HashRecuperation = hashCode; compte.IterationsRecuperation = iterationsCode;
            await _comptes.MettreAJourAsync(compte);

            if (_stockage.LireCompteMemorise() == compte.Id)
                _stockage.Oublier(); // l'ancienne session mémorisée n'est plus valable
            return nouveauCode;
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

    private static void VerifierMotDePasse(string motDePasse)
    {
        if (string.IsNullOrEmpty(motDePasse) || motDePasse.Length < LongueurMinimale)
            throw new AuthentificationException($"Le mot de passe doit contenir au moins {LongueurMinimale} caractères.");
    }

    private void VerifierPasBloque(string cle)
    {
        var reste = _limiteur.TempsRestant(cle);
        if (reste.HasValue)
            throw new AuthentificationException($"Trop de tentatives incorrectes. Réessayez dans {Math.Ceiling(reste.Value.TotalSeconds)} seconde(s).");
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
