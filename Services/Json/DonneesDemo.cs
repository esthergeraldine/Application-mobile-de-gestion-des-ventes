using VenteJour.Models;
using VenteJour.Services.Fake;

namespace VenteJour.Services.Json;

/// <summary>
/// Données d'exemple. Elles ne sont JAMAIS chargées automatiquement : un compte démarre à zéro.
/// Elles ne servent que quand l'utilisateur choisit « Charger des données de démonstration » dans Paramètres.
/// </summary>
public static class DonneesDemo
{
    /// <summary>False par défaut : les dépôts démarrent vides. (Seuls des tests peuvent le passer à true.)</summary>
    public static bool Actives { get; set; } = false;

    private static readonly Lazy<FakeDataStore> Source = new(() =>
    {
        var ancien = FakeDataStore.InclureDonneesDemo;
        FakeDataStore.InclureDonneesDemo = true;
        try { return new FakeDataStore(); }
        finally { FakeDataStore.InclureDonneesDemo = ancien; }
    });

    public static List<Produit> ProduitsDemo() => Source.Value.Produits.ToList();
    public static List<Vente> VentesDemo() => Source.Value.Ventes.ToList();

    // Valeurs initiales des dépôts : vides tant que Actives = false
    public static List<Produit> Produits() => Actives ? ProduitsDemo() : new List<Produit>();
    public static List<Vente> Ventes() => Actives ? VentesDemo() : new List<Vente>();
    public static Kiosque KiosqueInitial() => Actives ? Source.Value.Kiosque : new Kiosque { Id = 1 };
}
