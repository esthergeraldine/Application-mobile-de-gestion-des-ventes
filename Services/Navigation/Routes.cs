namespace VenteJour.Services;

public static class Routes
{
    // Pages racines (ShellContent) -> navigation absolue "//route"
    public const string Splash = "splash";
    public const string Connexion = "connexion";
    public const string Inscription = "inscription";
    public const string Accueil = "accueil";
    public const string Produits = "produits";
    public const string Historique = "historique";
    public const string Profil = "profil";

    // Pages empilées -> navigation relative "route?param=valeur"
    public const string NouvelleVente = "nouvellevente";
    public const string VentePerte = "venteperte";
    public const string Succes = "succes";
    public const string AjouterProduit = "ajouterproduit";
    public const string Detail = "detail";
    public const string Parametres = "parametres";
    public const string Kiosque = "kiosque";
    public const string MotDePasseOublie = "motdepasseoublie";
    public const string Classement = "classement";
    public const string Aide = "aide";
    public const string AideCategorie = "aidecategorie";
    public const string AideArticle = "aidearticle";
    public const string ContactSupport = "contactsupport";
    public const string SignalerProbleme = "signalerprobleme";

    public static string Racine(string route) => "//" + route;
}
