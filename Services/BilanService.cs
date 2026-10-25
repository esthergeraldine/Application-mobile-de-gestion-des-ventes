using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services;

public class BilanService : IBilanService
{
    private readonly IVenteRepository _ventes;
    public BilanService(IVenteRepository ventes) => _ventes = ventes;

    public async Task<Bilan> GetBilanAsync(DateTime jour)
        => StatistiquesCalculator.CalculerBilan(await _ventes.ListerAsync(), jour);
}
