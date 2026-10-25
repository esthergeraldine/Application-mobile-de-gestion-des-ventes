using System.Net.Http;
using System.Net;
using System.Text;

namespace VenteJour.Services.Api;

/// <summary>
/// Phase B : le FAUX SERVEUR (comme dans le TP). Aucune requête ne sort du téléphone :
/// ce gestionnaire intercepte les appels de HttpClient et fabrique la réponse.
/// Il simule la latence du réseau et, au choix, une panne de connexion.
/// Quand la vraie API existe, on retire ce gestionnaire (new HttpClient()) et rien d'autre ne change.
/// </summary>
public class FakeApiHandler : HttpMessageHandler
{
    /// <summary>Passer à true pour simuler une coupure réseau (réglage "Démonstration" dans Paramètres).</summary>
    public static bool SimulerPanne { get; set; }

    /// <summary>Latence simulée en millisecondes.</summary>
    public static int LatenceMs { get; set; } = 1000;

    private int _compteurDemandes;
    private int _compteurSignalements;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requete, CancellationToken ct)
    {
        await Task.Delay(LatenceMs, ct); // latence simulée

        if (SimulerPanne)
            throw new HttpRequestException("Pas de connexion");

        var chemin = (requete.RequestUri?.AbsolutePath ?? string.Empty).Trim('/').ToLowerInvariant();

        if (requete.Method == HttpMethod.Get && chemin == "taux")
            return Json(HttpStatusCode.OK, "{\"base\":\"XAF\",\"rates\":{\"EUR\":0.001524,\"USD\":0.00178}}");

        if (requete.Method == HttpMethod.Post && (chemin == "support/demandes" || chemin == "support/signalements"))
        {
            var corps = requete.Content == null ? string.Empty : await requete.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(corps))
                return Json(HttpStatusCode.BadRequest, "{\"erreur\":\"Corps de requête vide\"}");

            var reference = chemin == "support/demandes"
                ? $"SUP-{Interlocked.Increment(ref _compteurDemandes):000}"
                : $"SIG-{Interlocked.Increment(ref _compteurSignalements):000}";
            return Json(HttpStatusCode.Created, $"{{\"reference\":\"{reference}\"}}");
        }

        return Json(HttpStatusCode.NotFound, "{\"erreur\":\"Route inconnue\"}");
    }

    private static HttpResponseMessage Json(HttpStatusCode code, string json)
        => new(code) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
}
