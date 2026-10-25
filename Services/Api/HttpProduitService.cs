using System.Net.Http;
using System.Net.Http.Json;
using VenteJour.Models;

namespace VenteJour.Services.Api;

/// <summary>
/// SQUELETTE (non enregistré par défaut) : exemple d'implémentation HTTP de IProduitService.
/// Pour l'activer, voir MauiProgram.cs (section "Services de données").
/// Faire de même pour IVenteService, IBilanService et IAuthService.
/// </summary>
public class HttpProduitService : IProduitService
{
    private readonly HttpClient _http;
    public HttpProduitService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<Produit>> GetProduitsAsync()
        => await _http.GetFromJsonAsync<List<Produit>>("api/produits") ?? new List<Produit>();

    public async Task<Produit> GetProduitAsync(int id)
        => await _http.GetFromJsonAsync<Produit>($"api/produits/{id}");

    public async Task<Produit> AjouterAsync(Produit produit)
    {
        var reponse = await _http.PostAsJsonAsync("api/produits", produit);
        reponse.EnsureSuccessStatusCode();
        return await reponse.Content.ReadFromJsonAsync<Produit>();
    }

    public async Task MettreAJourAsync(Produit produit)
    {
        var reponse = await _http.PutAsJsonAsync($"api/produits/{produit.Id}", produit);
        reponse.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync()
        => await _http.GetFromJsonAsync<List<string>>("api/categories") ?? new List<string>();
}
