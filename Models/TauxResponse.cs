using System.Text.Json.Serialization;

namespace VenteJour.Models;

/// <summary>Réponse JSON de l'API de taux de change : {"base":"XAF","rates":{"EUR":0.001524,"USD":0.00178}}</summary>
public class TauxResponse
{
    [JsonPropertyName("base")]
    public string Base { get; set; } = "";

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Taux { get; set; } = new();
}

public class ResultatTaux
{
    public bool Disponible { get; set; }
    public decimal Taux { get; set; }

    /// <summary>True si le taux affiché est le dernier taux connu (pas de réseau ou réponse invalide).</summary>
    public bool DepuisCache { get; set; }

    public string Message { get; set; }
}
