using System.Net.Http;
using System.Net.Http.Json;
using VenteJour.Models;

namespace VenteJour.Services.Api;

/// <summary>Phase B : appels HTTP + désérialisation JSON. Reçoit son HttpClient par injection de dépendances.</summary>
public class ApiService : IApiService
{
    private sealed class ReferenceResponse
    {
        public string Reference { get; set; }
    }

    private readonly HttpClient _http;
    public ApiService(HttpClient http) => _http = http;

    // GET + contrôle du code + lecture + désérialisation
    public async Task<TauxResponse> GetTauxAsync(CancellationToken ct = default)
    {
        var reponse = await _http.GetFromJsonAsync<TauxResponse>("taux", ct);
        if (reponse == null || reponse.Taux == null || reponse.Taux.Count == 0)
            throw new InvalidOperationException("Réponse vide du serveur.");
        return reponse;
    }

    public async Task<string> EnvoyerDemandeAsync(DemandeSupport demande, CancellationToken ct = default)
        => await PosterAsync("support/demandes", new
        {
            demande.Nom, demande.Contact, demande.Sujet, demande.Categorie, demande.Description, demande.DateCreation
        }, ct);

    public async Task<string> EnvoyerSignalementAsync(SignalementProbleme signalement, CancellationToken ct = default)
        // Seul le NOM de la pièce jointe est transmis : le chemin local du téléphone ne doit jamais partir sur le réseau.
        // (Le téléversement réel du fichier sera ajouté avec la vraie API, en multipart.)
        => await PosterAsync("support/signalements", new
        {
            signalement.Type, signalement.Description, signalement.DateProbleme,
            signalement.ReferenceTransaction, signalement.PieceJointeNom, signalement.DateCreation
        }, ct);

    private async Task<string> PosterAsync(string route, object corps, CancellationToken ct)
    {
        using var reponse = await _http.PostAsJsonAsync(route, corps, ct);
        reponse.EnsureSuccessStatusCode(); // 4xx / 5xx → HttpRequestException

        var resultat = await reponse.Content.ReadFromJsonAsync<ReferenceResponse>(cancellationToken: ct);
        if (string.IsNullOrWhiteSpace(resultat?.Reference))
            throw new InvalidOperationException("Réponse vide du serveur.");
        return resultat.Reference;
    }
}
