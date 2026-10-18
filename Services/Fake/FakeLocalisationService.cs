namespace VenteJour.Services.Fake;

/// <summary>
/// SIMULATION : référentiel géographique réduit, fourni localement.
/// Le backend fournira le référentiel complet (régions, villes, quartiers) avec des identifiants stables.
/// Les villes sans quartiers connus proposent "Centre-ville" par défaut.
/// </summary>
public class FakeLocalisationService : ILocalisationService
{
    private static readonly string[] CentreVille = { "Centre-ville" };

    private static readonly Dictionary<string, Dictionary<string, string[]>> Donnees = new()
    {
        ["Adamaoua"] = new() { ["Ngaoundéré"] = CentreVille },
        ["Centre"] = new()
        {
            ["Yaoundé"] = new[] { "Bastos", "Biyem-Assi", "Briqueterie", "Emana", "Essos", "Melen", "Mokolo", "Mvan", "Mvog-Ada", "Nlongkak", "Nsimeyong" },
            ["Mbalmayo"] = CentreVille,
            ["Bafia"] = CentreVille
        },
        ["Est"] = new() { ["Bertoua"] = CentreVille },
        ["Extrême-Nord"] = new() { ["Maroua"] = CentreVille },
        ["Littoral"] = new()
        {
            ["Douala"] = new[] { "Akwa", "Bali", "Bepanda", "Bonabéri", "Bonanjo", "Bonapriso", "Deido", "Kotto", "Logbaba", "Logpom", "Makepe", "Ndokoti" },
            ["Edéa"] = CentreVille,
            ["Nkongsamba"] = CentreVille
        },
        ["Nord"] = new() { ["Garoua"] = CentreVille },
        ["Nord-Ouest"] = new()
        {
            ["Bamenda"] = new[] { "Commercial Avenue", "Mankon", "Nkwen", "Up Station" }
        },
        ["Ouest"] = new()
        {
            ["Bafoussam"] = new[] { "Djeleng", "Kamkop", "Tamdja" },
            ["Dschang"] = CentreVille
        },
        ["Sud"] = new() { ["Ebolowa"] = CentreVille, ["Kribi"] = CentreVille },
        ["Sud-Ouest"] = new() { ["Buea"] = CentreVille, ["Limbe"] = CentreVille }
    };

    public Task<IReadOnlyList<string>> GetRegionsAsync()
        => Task.FromResult<IReadOnlyList<string>>(Donnees.Keys.OrderBy(r => r, StringComparer.CurrentCulture).ToList());

    public Task<IReadOnlyList<string>> GetVillesAsync(string region)
    {
        IReadOnlyList<string> villes = region != null && Donnees.TryGetValue(region, out var v)
            ? v.Keys.OrderBy(x => x, StringComparer.CurrentCulture).ToList()
            : new List<string>();
        return Task.FromResult(villes);
    }

    public Task<IReadOnlyList<string>> GetQuartiersAsync(string region, string ville)
    {
        IReadOnlyList<string> quartiers = region != null && ville != null &&
                                          Donnees.TryGetValue(region, out var v) && v.TryGetValue(ville, out var q)
            ? q.OrderBy(x => x, StringComparer.CurrentCulture).ToList()
            : new List<string>();
        return Task.FromResult(quartiers);
    }
}
