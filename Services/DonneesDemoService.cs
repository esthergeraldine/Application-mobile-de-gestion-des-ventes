using VenteJour.Services.Json;

namespace VenteJour.Services;

public class DonneesDemoService : IDonneesDemoService
{
    private readonly IProduitRepository _produits;
    private readonly IVenteRepository _ventes;

    public DonneesDemoService(IProduitRepository produits, IVenteRepository ventes)
    {
        _produits = produits;
        _ventes = ventes;
    }

    public async Task ChargerAsync()
    {
        await _produits.RemplacerToutAsync(DonneesDemo.ProduitsDemo());
        await _ventes.RemplacerToutAsync(DonneesDemo.VentesDemo());
    }
}
