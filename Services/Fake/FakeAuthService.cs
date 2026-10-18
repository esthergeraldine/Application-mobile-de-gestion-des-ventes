using VenteJour.Models;

namespace VenteJour.Services.Fake;

public class FakeAuthService : IAuthService
{
    private readonly FakeDataStore _store;
    private Utilisateur _courant;

    public FakeAuthService(FakeDataStore store) => _store = store;

    public async Task<Utilisateur> ConnexionAsync(string identifiant, string motDePasse)
    {
        await Task.Delay(300);
        if (string.IsNullOrWhiteSpace(identifiant) || string.IsNullOrWhiteSpace(motDePasse))
            return null;
        _courant = _store.Utilisateur;
        return _courant;
    }

    public async Task<Utilisateur> InscriptionAsync(Utilisateur utilisateur, string motDePasse)
    {
        await Task.Delay(300);
        if (string.IsNullOrWhiteSpace(utilisateur.Nom))
            utilisateur.Nom = "Gérant(e)";
        _store.Utilisateur = utilisateur;
        _store.Kiosque = new Kiosque
        {
            Id = 1, Nom = utilisateur.NomKiosque, Responsable = utilisateur.Nom,
            Telephone = Domain.ValidationHelper.ChiffresTelephone(utilisateur.Telephone)
        };
        _courant = utilisateur;
        return utilisateur;
    }

    public Task<Utilisateur> GetUtilisateurCourantAsync() => Task.FromResult(_courant ?? _store.Utilisateur);

    public Task DeconnexionAsync()
    {
        _courant = null;
        return Task.CompletedTask;
    }
}
