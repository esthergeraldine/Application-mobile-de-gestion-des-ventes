using VenteJour.Models;

namespace VenteJour.Services.Json;

/// <summary>Phase A : produits stockés dans produits.json (dossier privé de l'application).</summary>
public class JsonProduitRepository : IProduitRepository
{
    private readonly JsonFichier<List<Produit>> _fichier;

    public JsonProduitRepository(string dossier, Func<List<Produit>> donneesInitiales = null)
        => _fichier = new JsonFichier<List<Produit>>(dossier, "produits.json", donneesInitiales);

    public async Task<List<Produit>> ListerAsync()
        => (await _fichier.LireAsync()).OrderBy(p => p.Id).ToList();

    public async Task<Produit> ObtenirAsync(int id)
        => (await _fichier.LireAsync()).FirstOrDefault(p => p.Id == id);

    public Task<Produit> AjouterAsync(Produit produit)
        => _fichier.TransformerAsync(liste =>
        {
            produit.Id = (liste.Count == 0 ? 0 : liste.Max(p => p.Id)) + 1;
            liste.Add(Copier(produit));
            return produit;
        });

    public Task MettreAJourAsync(Produit produit)
        => _fichier.TransformerAsync(liste =>
        {
            var index = liste.FindIndex(p => p.Id == produit.Id);
            if (index < 0)
                throw new InvalidOperationException("Produit introuvable.");
            liste[index] = Copier(produit);
            return true;
        });

    public Task RemplacerToutAsync(List<Produit> produits) => _fichier.RemplacerAsync(produits);

    private static Produit Copier(Produit p) => new()
    {
        Id = p.Id, Nom = p.Nom, Categorie = p.Categorie, PrixAchat = p.PrixAchat,
        PrixVente = p.PrixVente, Stock = p.Stock, Icone = p.Icone
    };
}
