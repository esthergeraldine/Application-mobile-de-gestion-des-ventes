using VenteJour.Models;

namespace VenteJour.Services.Fake;

/// <summary>
/// SIMULATION locale : la demande est enregistrée en mémoire sur l'appareil.
/// Rien n'est transmis à un serveur (Transmis = false) et l'interface doit le dire.
/// </summary>
public class FakeSupportService : ISupportService
{
    private readonly FakeDataStore _store;
    public FakeSupportService(FakeDataStore store) => _store = store;

    public bool EstSimulation => true;

    public async Task<ResultatEnvoi> EnvoyerDemandeAsync(DemandeSupport demande)
    {
        await Task.Delay(500);
        demande.Id = $"SUP-{_store.DemandesSupport.Count + 1:000}";
        demande.DateCreation = DateTime.Now;
        _store.DemandesSupport.Add(demande);
        return new ResultatEnvoi { Reference = demande.Id, Transmis = false };
    }

    public async Task<ResultatEnvoi> EnvoyerSignalementAsync(SignalementProbleme signalement)
    {
        await Task.Delay(500);
        signalement.Id = $"SIG-{_store.Signalements.Count + 1:000}";
        signalement.DateCreation = DateTime.Now;
        _store.Signalements.Add(signalement);
        return new ResultatEnvoi { Reference = signalement.Id, Transmis = false };
    }
}
