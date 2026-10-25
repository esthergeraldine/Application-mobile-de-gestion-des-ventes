using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>
/// Phase B : contrat de l'API distante. Aujourd'hui répondue par un faux serveur (FakeApiHandler),
/// demain par la vraie API ASP.NET Core : seule l'injection de HttpClient change dans MauiProgram.cs.
/// </summary>
public interface IApiService
{
    Task<TauxResponse> GetTauxAsync(CancellationToken ct = default);
    Task<string> EnvoyerDemandeAsync(DemandeSupport demande, CancellationToken ct = default);
    Task<string> EnvoyerSignalementAsync(SignalementProbleme signalement, CancellationToken ct = default);
}

public interface ITauxChangeService
{
    /// <summary>Taux FCFA → EUR. Ne lève pas d'exception réseau : retourne le dernier taux connu ou "indisponible".</summary>
    Task<ResultatTaux> ObtenirTauxEurAsync(CancellationToken ct = default);
}

/// <summary>Petit cache clé/valeur local (Preferences sur le téléphone).</summary>
public interface ICacheLocal
{
    bool TryLire(string cle, out double valeur);
    void Ecrire(string cle, double valeur);
}
