using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>Erreur d'authentification dont le message est destiné à être affiché à l'utilisateur.</summary>
public class AuthentificationException : Exception
{
    public AuthentificationException(string message) : base(message) { }
}

/// <summary>Résultat d'une inscription : l'utilisateur connecté et son code de récupération (affiché UNE seule fois).</summary>
public class ResultatInscription
{
    public Utilisateur Utilisateur { get; set; }
    public string CodeRecuperation { get; set; }
}

public interface IAuthService
{
    /// <summary>Utilisateur connecté, ou null.</summary>
    Utilisateur UtilisateurCourant { get; }

    /// <summary>Lève AuthentificationException si le compte n'existe pas ou si le mot de passe est incorrect.</summary>
    Task<Utilisateur> ConnexionAsync(string identifiant, string motDePasse, bool memoriser);

    /// <summary>Crée le compte (refusé si l'e-mail ou le téléphone existe déjà), ouvre la session et crée le kiosque vide.</summary>
    Task<ResultatInscription> InscriptionAsync(string identifiant, string nomKiosque, string telephone, string motDePasse);

    /// <summary>
    /// Mot de passe oublié : vérifie le code de récupération, change le mot de passe et retourne un NOUVEAU code
    /// (l'ancien n'est plus valable). Lève AuthentificationException si le compte, le code ou le mot de passe est invalide.
    /// </summary>
    Task<string> ReinitialiserMotDePasseAsync(string identifiant, string codeRecuperation, string nouveauMotDePasse);

    /// <summary>Rouvre la session mémorisée (« Se souvenir de moi »). Null s'il n'y en a pas.</summary>
    Task<Utilisateur> RestaurerSessionAsync();

    Task DeconnexionAsync();
}

/// <summary>Session en cours : détermine à quel compte appartiennent les données lues et écrites.</summary>
public interface ISession
{
    Utilisateur Courant { get; }
    void Ouvrir(Utilisateur utilisateur);
    void Fermer();
}

/// <summary>Mémorise (sur le téléphone) le compte à rouvrir au prochain lancement.</summary>
public interface IStockageSession
{
    int? LireCompteMemorise();
    void MemoriserCompte(int compteId);
    void Oublier();
}

public interface ICompteRepository
{
    Task<List<CompteLocal>> ListerAsync();

    /// <summary>Ajoute le compte et lui attribue un identifiant unique.</summary>
    Task<CompteLocal> AjouterAsync(CompteLocal compte);

    Task MettreAJourAsync(CompteLocal compte);
}

public interface IDonneesDemoService
{
    /// <summary>Remplace les produits et ventes du compte courant par des données d'exemple (captures d'écran, démonstration).</summary>
    Task ChargerAsync();
}
