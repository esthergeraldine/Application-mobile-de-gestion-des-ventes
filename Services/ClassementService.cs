using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>Le classement est recalculé à chaque appel à partir des ventes enregistrées (jamais mis en cache).</summary>
public class ClassementService : IClassementService
{
    private readonly IVenteRepository _ventes;
    private readonly IProduitRepository _produits;

    public ClassementService(IVenteRepository ventes, IProduitRepository produits)
    {
        _ventes = ventes;
        _produits = produits;
    }

    public async Task<IReadOnlyList<ClassementProduit>> GetClassementAsync(CritereClassement critere, PlageDates plage, int? limite = null)
        => StatistiquesCalculator.Classer(await _ventes.ListerAsync(), await _produits.ListerAsync(), plage, critere, limite);
}
