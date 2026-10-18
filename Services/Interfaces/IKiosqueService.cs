using VenteJour.Models;

namespace VenteJour.Services;

public interface IKiosqueService
{
    /// <summary>True tant que l'enregistrement reste local (aucun serveur contacté).</summary>
    bool EstSimulation { get; }

    Task<Kiosque> GetKiosqueAsync();
    Task EnregistrerAsync(Kiosque kiosque);
}

public interface ILocalisationService
{
    Task<IReadOnlyList<string>> GetRegionsAsync();
    Task<IReadOnlyList<string>> GetVillesAsync(string region);
    Task<IReadOnlyList<string>> GetQuartiersAsync(string region, string ville);
}
