using VenteJour.Models;

namespace VenteJour.Services;

public class SessionUtilisateur : ISession
{
    public Utilisateur Courant { get; private set; }

    public void Ouvrir(Utilisateur utilisateur) => Courant = utilisateur;

    public void Fermer() => Courant = null;
}
