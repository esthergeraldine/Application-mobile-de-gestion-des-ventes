using VenteJour.Models;

namespace VenteJour.Services;

public interface IProduitService
{
    Task<IReadOnlyList<Produit>> GetProduitsAsync();
    Task<Produit> GetProduitAsync(int id);
    Task<Produit> AjouterAsync(Produit produit);
    Task MettreAJourAsync(Produit produit);
    Task<IReadOnlyList<string>> GetCategoriesAsync();
}
