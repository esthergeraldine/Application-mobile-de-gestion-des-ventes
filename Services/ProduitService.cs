using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>Phase A : logique produits au-dessus d'un IProduitRepository (JSON, puis SQLite en phase C).</summary>
public class ProduitService : IProduitService
{
    private readonly IProduitRepository _produits;
    public ProduitService(IProduitRepository produits) => _produits = produits;

    public async Task<IReadOnlyList<Produit>> GetProduitsAsync() => await _produits.ListerAsync();

    public Task<Produit> GetProduitAsync(int id) => _produits.ObtenirAsync(id);

    public Task<Produit> AjouterAsync(Produit produit) => _produits.AjouterAsync(produit);

    public Task MettreAJourAsync(Produit produit) => _produits.MettreAJourAsync(produit);

    public async Task<IReadOnlyList<string>> GetCategoriesAsync()
    {
        var existantes = (await _produits.ListerAsync()).Select(p => p.Categorie);
        return new[] { "Boissons", "Aliments", "Hygiène", "Autres" }
            .Concat(existantes)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .ToList();
    }
}
