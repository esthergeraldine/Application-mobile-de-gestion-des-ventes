using VenteJour.Models;

namespace VenteJour.Services.Fake;

public class FakeProduitService : IProduitService
{
    private readonly FakeDataStore _store;
    public FakeProduitService(FakeDataStore store) => _store = store;

    public Task<IReadOnlyList<Produit>> GetProduitsAsync()
        => Task.FromResult<IReadOnlyList<Produit>>(_store.Produits.Select(Copier).ToList());

    public Task<Produit> GetProduitAsync(int id)
    {
        var p = _store.Produits.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(p == null ? null : Copier(p));
    }

    public Task<Produit> AjouterAsync(Produit produit)
    {
        produit.Id = _store.NouvelIdProduit();
        _store.Produits.Add(Copier(produit));
        return Task.FromResult(produit);
    }

    public Task MettreAJourAsync(Produit produit)
    {
        var index = _store.Produits.FindIndex(x => x.Id == produit.Id);
        if (index >= 0)
            _store.Produits[index] = Copier(produit);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<string>> GetCategoriesAsync()
    {
        var categories = new[] { "Boissons", "Aliments", "Hygiène", "Autres" }
            .Concat(_store.Produits.Select(p => p.Categorie))
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .ToList();
        return Task.FromResult<IReadOnlyList<string>>(categories);
    }

    private static Produit Copier(Produit p) => new()
    {
        Id = p.Id, Nom = p.Nom, Categorie = p.Categorie, PrixAchat = p.PrixAchat,
        PrixVente = p.PrixVente, Stock = p.Stock, Icone = p.Icone
    };
}
