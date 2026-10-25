using VenteJour.Models;

namespace VenteJour.Services.Sqlite;

/// <summary>Phase C : produits dans la table Produits (remplace JsonProduitRepository, même interface).</summary>
public class SqliteProduitRepository : IProduitRepository
{
    private readonly SqliteBaseDeDonnees _base;
    public SqliteProduitRepository(SqliteBaseDeDonnees baseDeDonnees) => _base = baseDeDonnees;

    public async Task<List<Produit>> ListerAsync()
    {
        var db = await _base.ObtenirAsync();
        var lignes = await db.Table<ProduitEntite>().OrderBy(p => p.Id).ToListAsync();
        return lignes.Select(e => e.VersModele()).ToList();
    }

    public async Task<Produit> ObtenirAsync(int id)
    {
        var db = await _base.ObtenirAsync();
        return (await db.FindAsync<ProduitEntite>(id))?.VersModele();
    }

    public async Task<Produit> AjouterAsync(Produit produit)
    {
        var db = await _base.ObtenirAsync();
        var entite = produit.VersEntite();
        entite.Id = 0; // identifiant attribué automatiquement par SQLite (AutoIncrement)
        await db.InsertAsync(entite);
        produit.Id = entite.Id;
        return produit;
    }

    public async Task MettreAJourAsync(Produit produit)
    {
        var db = await _base.ObtenirAsync();
        if (await db.UpdateAsync(produit.VersEntite()) == 0)
            throw new InvalidOperationException("Produit introuvable.");
    }

    public async Task RemplacerToutAsync(List<Produit> produits)
    {
        var db = await _base.ObtenirAsync();
        await db.RunInTransactionAsync(conn =>
        {
            conn.DeleteAll<ProduitEntite>();
            foreach (var p in produits)
                conn.InsertOrReplace(p.VersEntite());
        });
    }
}
