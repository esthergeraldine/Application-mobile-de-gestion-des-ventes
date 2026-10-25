using SQLite;
using VenteJour.Models;
using VenteJour.Services.Json;

namespace VenteJour.Services.Sqlite;

/// <summary>
/// Phase C : UNE base SQLite (ventejour.db3) partagée par les trois dépôts. Création paresseuse au premier accès
/// (comme InitAsync dans SqliteDepenseRepository du TP) : connexion, création des tables, puis remplissage initial.
/// </summary>
public sealed class SqliteBaseDeDonnees
{
    /// <summary>Version du schéma, stockée dans la base (PRAGMA user_version) pour ne remplir qu'UNE fois.</summary>
    private const int VersionSchema = 1;

    private readonly Func<string> _fournisseurChemin;
    private string _cheminOuvert;
    private readonly string _dossierJson;
    private readonly Func<List<Produit>> _produitsDemo;
    private readonly Func<List<Vente>> _ventesDemo;
    private readonly Func<Kiosque> _kiosqueDemo;
    private readonly SemaphoreSlim _verrou = new(1, 1);
    private SQLiteAsyncConnection _db;

    /// <summary>Base à chemin dynamique : le fichier peut changer (une base par compte connecté).</summary>
    public SqliteBaseDeDonnees(Func<string> fournisseurChemin, string dossierJson = null,
        Func<List<Produit>> produitsDemo = null, Func<List<Vente>> ventesDemo = null, Func<Kiosque> kiosqueDemo = null)
    {
        _fournisseurChemin = fournisseurChemin;
        _dossierJson = dossierJson;
        _produitsDemo = produitsDemo;
        _ventesDemo = ventesDemo;
        _kiosqueDemo = kiosqueDemo;
    }

    /// <param name="chemin">Fichier de la base (ex. dossier de l'application + "ventejour.db3").</param>
    /// <param name="dossierJson">Dossier des fichiers JSON de la phase A : s'ils existent, leurs données sont importées.</param>
    public SqliteBaseDeDonnees(string chemin, string dossierJson = null,
        Func<List<Produit>> produitsDemo = null, Func<List<Vente>> ventesDemo = null, Func<Kiosque> kiosqueDemo = null)
    {
        _fournisseurChemin = () => chemin;
        _dossierJson = dossierJson;
        _produitsDemo = produitsDemo;
        _ventesDemo = ventesDemo;
        _kiosqueDemo = kiosqueDemo;
    }

    public async Task<SQLiteAsyncConnection> ObtenirAsync()
    {
        var chemin = _fournisseurChemin();
        if (_db != null && _cheminOuvert == chemin)
            return _db;

        await _verrou.WaitAsync();
        try
        {
            chemin = _fournisseurChemin();
            if (_db != null && _cheminOuvert == chemin)
                return _db;

            if (_db != null) // un autre compte vient de se connecter : on ferme la base du précédent
            {
                await _db.CloseAsync();
                _db = null;
            }

            var dossier = Path.GetDirectoryName(chemin);
            if (!string.IsNullOrEmpty(dossier))
                Directory.CreateDirectory(dossier);

            var db = new SQLiteAsyncConnection(chemin);
            await db.CreateTableAsync<ProduitEntite>();
            await db.CreateTableAsync<VenteEntite>();
            await db.CreateTableAsync<LigneVenteEntite>();
            await db.CreateTableAsync<KiosqueEntite>();

            var version = await db.ExecuteScalarAsync<int>("PRAGMA user_version");
            if (version < VersionSchema)
            {
                await RemplirAsync(db);
                await db.ExecuteAsync($"PRAGMA user_version = {VersionSchema}");
            }

            _db = db;
            _cheminOuvert = chemin;
            return db;
        }
        finally
        {
            _verrou.Release();
        }
    }

    private async Task RemplirAsync(SQLiteAsyncConnection db)
    {
        List<Produit> produits;
        List<Vente> ventes;
        Kiosque kiosque;

        // Les JSON de la phase A existent : on migre les données déjà saisies, on ne les perd pas.
        if (_dossierJson != null && File.Exists(Path.Combine(_dossierJson, "produits.json")))
        {
            produits = await new JsonProduitRepository(_dossierJson).ListerAsync();
            ventes = await new JsonVenteRepository(_dossierJson).ListerAsync();
            kiosque = File.Exists(Path.Combine(_dossierJson, "kiosque.json"))
                ? await new JsonKiosqueRepository(_dossierJson).ObtenirAsync()
                : _kiosqueDemo?.Invoke();
        }
        else
        {
            produits = _produitsDemo?.Invoke() ?? new List<Produit>();
            ventes = _ventesDemo?.Invoke() ?? new List<Vente>();
            kiosque = _kiosqueDemo?.Invoke();
        }

        // Une seule transaction : soit tout est importé, soit rien (pas de base à moitié remplie)
        await db.RunInTransactionAsync(conn =>
        {
            foreach (var p in produits)
                conn.InsertOrReplace(p.VersEntite()); // InsertOrReplace conserve les identifiants existants

            foreach (var v in ventes)
            {
                conn.InsertOrReplace(v.VersEntite());
                foreach (var ligne in v.LignesVersEntites())
                    conn.Insert(ligne);
            }

            if (kiosque != null)
                conn.InsertOrReplace(kiosque.VersEntite());
        });
    }
}
