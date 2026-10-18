using VenteJour.Models;

namespace VenteJour.Services;

public interface IAuthService
{
    Task<Utilisateur> ConnexionAsync(string identifiant, string motDePasse);
    Task<Utilisateur> InscriptionAsync(Utilisateur utilisateur, string motDePasse);
    Task<Utilisateur> GetUtilisateurCourantAsync();
    Task DeconnexionAsync();
}
