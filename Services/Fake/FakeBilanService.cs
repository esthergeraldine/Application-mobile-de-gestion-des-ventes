using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services.Fake;

public class FakeBilanService : IBilanService
{
    private readonly FakeDataStore _store;
    public FakeBilanService(FakeDataStore store) => _store = store;

    public Task<Bilan> GetBilanAsync(DateTime jour)
        => Task.FromResult(StatistiquesCalculator.CalculerBilan(_store.Ventes, jour));
}
