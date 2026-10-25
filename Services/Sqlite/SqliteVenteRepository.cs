using VenteJour.Models;

namespace VenteJour.Services.Sqlite;

/// <summary>
/// Phase C : une vente = une ligne de la table Ventes + ses articles dans LignesVente.
/// Chaque écriture est une transaction : une vente n'est jamais enregistrée à moitié.
/// </summary>
public class SqliteVenteRepository : IVenteRepository
{
    private readonly SqliteBaseDeDonnees _base;
    public SqliteVenteRepository(SqliteBaseDeDonnees baseDeDonnees) => _base = baseDeDonnees;

    public async Task<List<Vente>> ListerAsync()
    {
        var db = await _base.ObtenirAsync();
        var ventes = await db.Table<VenteEntite>().OrderBy(v => v.Date).ToListAsync();
        var lignes = (await db.Table<LigneVenteEntite>().ToListAsync()).ToLookup(l => l.VenteId);
        return ventes.Select(v => v.VersModele(lignes[v.Id])).ToList();
    }

    public async Task<Vente> ObtenirAsync(string id)
    {
        var db = await _base.ObtenirAsync();
        var entite = await db.FindAsync<VenteEntite>(id);
        if (entite == null)
            return null;
        var lignes = await db.Table<LigneVenteEntite>().Where(l => l.VenteId == id).ToListAsync();
        return entite.VersModele(lignes);
    }

    public async Task<Vente> AjouterAsync(Vente vente)
    {
        var db = await _base.ObtenirAsync();
        await db.RunInTransactionAsync(conn =>
        {
            // Numéro suivant calculé DANS la transaction : pas de doublon possible
            int dernier = conn.ExecuteScalar<int>("SELECT IFNULL(MAX(CAST(SUBSTR(Id, 4) AS INTEGER)), 0) FROM Ventes");
            vente.Id = $"TRX{dernier + 1:000}";

            conn.Insert(vente.VersEntite());
            foreach (var ligne in vente.LignesVersEntites())
                conn.Insert(ligne);
        });
        return vente;
    }

    public async Task MettreAJourAsync(Vente vente)
    {
        var db = await _base.ObtenirAsync();
        await db.RunInTransactionAsync(conn =>
        {
            if (conn.Update(vente.VersEntite()) == 0)
                throw new InvalidOperationException("Transaction introuvable."); // annule la transaction

            conn.Execute("DELETE FROM LignesVente WHERE VenteId = ?", vente.Id);
            foreach (var ligne in vente.LignesVersEntites())
                conn.Insert(ligne);
        });
    }

    public async Task RemplacerToutAsync(List<Vente> ventes)
    {
        var db = await _base.ObtenirAsync();
        await db.RunInTransactionAsync(conn =>
        {
            conn.DeleteAll<LigneVenteEntite>();
            conn.DeleteAll<VenteEntite>();
            foreach (var v in ventes)
            {
                conn.InsertOrReplace(v.VersEntite());
                foreach (var ligne in v.LignesVersEntites())
                    conn.Insert(ligne);
            }
        });
    }
}
