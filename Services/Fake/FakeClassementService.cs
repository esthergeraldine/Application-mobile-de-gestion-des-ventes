using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services.Fake;

/// <summary>SIMULATION locale : recalcule le classement à chaque appel à partir des ventes du FakeDataStore.</summary>
public class FakeClassementService : IClassementService
{
    private readonly FakeDataStore _store;
    public FakeClassementService(FakeDataStore store) => _store = store;

    public Task<IReadOnlyList<ClassementProduit>> GetClassementAsync(CritereClassement critere, PlageDates plage, int? limite = null)
    {
        // Copie défensive : le calcul porte toujours sur l'état courant des ventes.
        var ventes = _store.Ventes.ToList();
        var produits = _store.Produits.ToList();
        return Task.FromResult(StatistiquesCalculator.Classer(ventes, produits, plage, critere, limite));
    }
}
