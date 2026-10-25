using VenteJour.Models;

namespace VenteJour.Services.Sqlite;

/// <summary>Phase C : le kiosque est une ligne unique (Id = 1) de la table Kiosque.</summary>
public class SqliteKiosqueRepository : IKiosqueRepository
{
    private readonly SqliteBaseDeDonnees _base;
    public SqliteKiosqueRepository(SqliteBaseDeDonnees baseDeDonnees) => _base = baseDeDonnees;

    public async Task<Kiosque> ObtenirAsync()
    {
        var db = await _base.ObtenirAsync();
        var entite = await db.FindAsync<KiosqueEntite>(1);
        return entite?.VersModele() ?? new Kiosque { Id = 1 };
    }

    public async Task EnregistrerAsync(Kiosque kiosque)
    {
        var db = await _base.ObtenirAsync();
        await db.InsertOrReplaceAsync(kiosque.VersEntite());
    }
}
