using VenteJour.Models;

namespace VenteJour.Services.Api;

/// <summary>
/// Phase B : le support passe par l'API (simulée). Si le réseau est coupé, l'exception HttpRequestException
/// remonte jusqu'au ViewModel, qui affiche « non envoyé » et conserve la saisie : jamais de faux « envoyé ».
/// </summary>
public class ApiSupportService : ISupportService
{
    private readonly IApiService _api;
    private readonly ApiOptions _options;

    public ApiSupportService(IApiService api, ApiOptions options)
    {
        _api = api;
        _options = options;
    }

    public bool EstSimulation => _options.Simulation;

    public async Task<ResultatEnvoi> EnvoyerDemandeAsync(DemandeSupport demande)
    {
        demande.DateCreation = DateTime.Now;
        var reference = await _api.EnvoyerDemandeAsync(demande);
        return new ResultatEnvoi { Reference = reference, Transmis = !_options.Simulation };
    }

    public async Task<ResultatEnvoi> EnvoyerSignalementAsync(SignalementProbleme signalement)
    {
        signalement.DateCreation = DateTime.Now;
        var reference = await _api.EnvoyerSignalementAsync(signalement);
        return new ResultatEnvoi { Reference = reference, Transmis = !_options.Simulation };
    }
}
