using System.Diagnostics;
using System.Text.Json;

namespace VenteJour.Services.Json;

/// <summary>
/// Lecture / écriture sécurisée d'UN fichier JSON (comme JsonSerializer dans le TP, avec en plus :
/// verrou contre les accès simultanés, écriture atomique, et protection contre un fichier corrompu).
/// </summary>
internal sealed class JsonFichier<T> where T : new()
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        IgnoreReadOnlyProperties = true // les propriétés calculées (Total, Benefice...) ne sont pas stockées
    };

    private readonly string _chemin;
    private readonly Func<T> _valeurInitiale;
    private readonly SemaphoreSlim _verrou = new(1, 1);

    public JsonFichier(string dossier, string nomFichier, Func<T> valeurInitiale)
    {
        Directory.CreateDirectory(dossier);
        _chemin = Path.Combine(dossier, nomFichier);
        _valeurInitiale = valeurInitiale ?? (() => new T());
    }

    public string Chemin => _chemin;

    /// <summary>Retourne une copie fraîche des données (jamais d'objet partagé entre deux appels).</summary>
    public async Task<T> LireAsync()
    {
        await _verrou.WaitAsync();
        try
        {
            return await ChargerAsync();
        }
        finally
        {
            _verrou.Release();
        }
    }

    /// <summary>Lit, applique la modification, puis réécrit le fichier, le tout sans interruption possible.</summary>
    public async Task<TResultat> TransformerAsync<TResultat>(Func<T, TResultat> modification)
    {
        await _verrou.WaitAsync();
        try
        {
            var donnees = await ChargerAsync();
            var resultat = modification(donnees);
            await EcrireAsync(donnees);
            return resultat;
        }
        finally
        {
            _verrou.Release();
        }
    }

    public async Task RemplacerAsync(T donnees)
    {
        await _verrou.WaitAsync();
        try
        {
            await EcrireAsync(donnees);
        }
        finally
        {
            _verrou.Release();
        }
    }

    private async Task<T> ChargerAsync()
    {
        // Premier lancement : le fichier n'existe pas encore → valeurs initiales (données de démonstration ou vide)
        if (!File.Exists(_chemin))
        {
            var initial = _valeurInitiale();
            await EcrireAsync(initial);
            return initial;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_chemin);
            return JsonSerializer.Deserialize<T>(json, Options) ?? new T();
        }
        catch (JsonException ex)
        {
            // Fichier illisible : on le met de côté (pour ne rien perdre) et l'application repart d'un état vide.
            var copie = $"{_chemin}.corrompu-{DateTime.Now:yyyyMMddHHmmss}";
            File.Move(_chemin, copie);
            Debug.WriteLine($"[JSON] Fichier corrompu déplacé vers {copie} : {ex.Message}");
            var vide = new T();
            await EcrireAsync(vide);
            return vide;
        }
    }

    private async Task EcrireAsync(T donnees)
    {
        // Écriture atomique : fichier temporaire puis remplacement, pour ne jamais laisser un fichier à moitié écrit
        var temporaire = _chemin + ".tmp";
        var json = JsonSerializer.Serialize(donnees, Options);
        await File.WriteAllTextAsync(temporaire, json);
        File.Move(temporaire, _chemin, overwrite: true);
    }
}
