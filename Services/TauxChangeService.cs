using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>
/// Phase B : taux FCFA → EUR via l'API, avec repli sur le dernier taux connu quand le réseau est coupé
/// (même logique que ConvertirAsync du TP : IsBusy, message « Hors ligne », cache).
/// </summary>
public class TauxChangeService : ITauxChangeService
{
    private const string CleCache = "taux_eur";

    private readonly IApiService _api;
    private readonly ICacheLocal _cache;

    public TauxChangeService(IApiService api, ICacheLocal cache)
    {
        _api = api;
        _cache = cache;
    }

    public async Task<ResultatTaux> ObtenirTauxEurAsync(CancellationToken ct = default)
    {
        try
        {
            var reponse = await _api.GetTauxAsync(ct);
            if (!reponse.Taux.TryGetValue("EUR", out var eur) || eur <= 0)
                throw new InvalidOperationException("Taux EUR absent de la réponse.");

            _cache.Ecrire(CleCache, (double)eur); // dernier taux connu
            return new ResultatTaux { Disponible = true, Taux = eur };
        }
        catch (HttpRequestException)
        {
            return DernierTauxConnu("Hors ligne : dernier taux connu.");
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) // délai dépassé
        {
            return DernierTauxConnu("Délai dépassé : dernier taux connu.");
        }
        catch (InvalidOperationException)
        {
            return DernierTauxConnu("Réponse du serveur invalide : dernier taux connu.");
        }
        catch (System.Text.Json.JsonException)
        {
            return DernierTauxConnu("Réponse du serveur illisible : dernier taux connu.");
        }
    }

    private ResultatTaux DernierTauxConnu(string messageAvecCache)
    {
        if (_cache.TryLire(CleCache, out var valeur) && valeur > 0)
            return new ResultatTaux { Disponible = true, Taux = (decimal)valeur, DepuisCache = true, Message = messageAvecCache };

        return new ResultatTaux { Disponible = false, Message = "Hors ligne : aucun taux de change connu." };
    }
}
