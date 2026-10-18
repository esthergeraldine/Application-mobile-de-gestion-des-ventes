using VenteJour.Models;

namespace VenteJour.Services.Fake;

/// <summary>SIMULATION locale : les informations sont conservées en mémoire sur l'appareil, aucun serveur n'est contacté.</summary>
public class FakeKiosqueService : IKiosqueService
{
    private readonly FakeDataStore _store;
    public FakeKiosqueService(FakeDataStore store) => _store = store;

    public bool EstSimulation => true;

    public async Task<Kiosque> GetKiosqueAsync()
    {
        await Task.Delay(150);
        return Copier(_store.Kiosque);
    }

    public async Task EnregistrerAsync(Kiosque kiosque)
    {
        await Task.Delay(400);
        _store.Kiosque = Copier(kiosque);
        // Cohérence avec le compte affiché ailleurs dans l'application
        _store.Utilisateur.NomKiosque = kiosque.Nom;
        _store.Utilisateur.Nom = kiosque.Responsable;
        _store.Utilisateur.Telephone = kiosque.Telephone;
    }

    private static Kiosque Copier(Kiosque k) => new()
    {
        Id = k.Id, Nom = k.Nom, Responsable = k.Responsable, Telephone = k.Telephone,
        Region = k.Region, Ville = k.Ville, Quartier = k.Quartier, AdressePrecise = k.AdressePrecise
    };
}
