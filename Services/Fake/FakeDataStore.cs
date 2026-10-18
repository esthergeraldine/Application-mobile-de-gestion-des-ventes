using VenteJour.Models;

namespace VenteJour.Services.Fake;

/// <summary>
/// Source unique des données de démonstration (en mémoire).
/// Partagée par les services fictifs afin qu'une vente enregistrée
/// mette à jour le stock, le bilan, l'historique et le classement.
/// Disparaît le jour où les services HTTP remplacent les services fictifs.
/// </summary>
public class FakeDataStore
{
    /// <summary>Mettre à false pour tester l'écran "Aucun produit pour le moment".</summary>
    public static bool InclureDonneesDemo { get; set; } = true;

    public List<Produit> Produits { get; } = new();
    public List<Vente> Ventes { get; } = new();
    public List<DemandeSupport> DemandesSupport { get; } = new();
    public List<SignalementProbleme> Signalements { get; } = new();

    public Kiosque Kiosque { get; set; } = new()
    {
        Id = 1,
        Nom = "Le Petit Marché",
        Responsable = "Esther Geraldine",
        Telephone = "699123456",
        Region = "Littoral",
        Ville = "Douala",
        Quartier = "Akwa",
        AdressePrecise = "Rue des Manguiers, près du carrefour"
    };
    public Utilisateur Utilisateur { get; set; } = new()
    {
        Id = 1,
        Nom = "Esther Geraldine",
        NomKiosque = "Le petit marché",
        Email = "esther@ventejour.app",
        Telephone = "+237 6 00 00 00 00"
    };

    private int _prochainIdProduit = 1;
    private int _prochainNumVente = 1;
    private static readonly string[] Modes = { "Espèces", "MTN Mobile Money", "Orange Money" };

    public FakeDataStore()
    {
        if (InclureDonneesDemo)
            Peupler();
    }

    public int NouvelIdProduit() => _prochainIdProduit++;
    public string NouvelIdVente() => $"TRX{_prochainNumVente++:000}";

    private Produit AjouterProduit(string nom, string categorie, int achat, int vente, int stock, string icone)
    {
        var p = new Produit
        {
            Id = NouvelIdProduit(), Nom = nom, Categorie = categorie,
            PrixAchat = achat, PrixVente = vente, Stock = stock, Icone = icone
        };
        Produits.Add(p);
        return p;
    }

    private void AjouterVente(DateTime date, string mode, params (Produit produit, int quantite)[] lignes)
    {
        var v = new Vente { Id = NouvelIdVente(), Date = date, ModeReglement = mode };
        foreach (var (p, q) in lignes)
        {
            v.Lignes.Add(new LigneVente
            {
                ProduitId = p.Id, NomProduit = p.Nom, Icone = p.Icone, Quantite = q,
                PrixAchatUnitaire = p.PrixAchat, PrixVenteUnitaire = p.PrixVente
            });
        }
        Ventes.Add(v);
    }

    private void Peupler()
    {
        var eau = AjouterProduit("Eau minérale 50cl", "Boissons", 500, 800, 20, "drop");
        var biscuits = AjouterProduit("Biscuits", "Aliments", 450, 700, 12, "cookie");
        var jusO = AjouterProduit("Jus d'orange", "Boissons", 350, 500, 20, "cup");
        var pain = AjouterProduit("Pain", "Aliments", 200, 300, 30, "bread");
        var jusF = AjouterProduit("Jus de fruits", "Boissons", 1000, 1200, 15, "cup");
        AjouterProduit("Jus d'orange 1L", "Boissons", 900, 800, 8, "cup"); // vendu à perte (démo)
        AjouterProduit("Soda 33cl", "Boissons", 350, 500, 24, "cup");
        AjouterProduit("Lait concentré", "Aliments", 300, 400, 18, "box");
        AjouterProduit("Sucre 1kg", "Aliments", 700, 850, 10, "box");
        AjouterProduit("Spaghetti 500g", "Aliments", 350, 450, 16, "box");
        AjouterProduit("Savon", "Hygiène", 250, 350, 22, "box");
        AjouterProduit("Jus de gingembre", "Boissons", 250, 400, 14, "cup");

        var today = DateTime.Today;
        var cinq = new[] { eau, biscuits, jusO, pain, jusF };

        // Transactions anciennes (TRX002 = écran "Détail de la transaction")
        AjouterVente(today.AddDays(-12).AddHours(10).AddMinutes(20), Modes[0], (pain, 2));
        AjouterVente(new DateTime(2026, 10, 6, 11, 41, 0), "MTN Mobile Money", (jusF, 2));

        // Hier : 11 900 FCFA de recette => aujourd'hui = +30 %
        AjouterVente(today.AddDays(-1).AddHours(8).AddMinutes(10), Modes[0], (pain, 4));
        AjouterVente(today.AddDays(-1).AddHours(9).AddMinutes(30), Modes[1], (eau, 3));
        AjouterVente(today.AddDays(-1).AddHours(11), Modes[0], (jusO, 4), (pain, 2));
        AjouterVente(today.AddDays(-1).AddHours(13).AddMinutes(15), Modes[2], (biscuits, 3));
        AjouterVente(today.AddDays(-1).AddHours(15), Modes[0], (eau, 3), (pain, 2));
        AjouterVente(today.AddDays(-1).AddHours(17).AddMinutes(20), Modes[1], (pain, 2));

        // Jours précédents (génération déterministe)
        for (int d = 2; d <= 13; d++)
        {
            for (int i = 0; i < 6; i++)
            {
                var p = cinq[(d + i) % 5];
                int q = 1 + ((d * 3 + i * 2) % 4);
                AjouterVente(today.AddDays(-d).AddHours(8 + i * 2).AddMinutes(5 * d), Modes[(d + i) % 3], (p, q));
            }
        }

        // Aujourd'hui : 12 transactions, 27 articles, 15 500 FCFA, bénéfice 5 200 FCFA (33,5 %)
        AjouterVente(today.AddHours(8).AddMinutes(15), Modes[0], (pain, 3));
        AjouterVente(today.AddHours(8).AddMinutes(40), Modes[0], (pain, 2));
        AjouterVente(today.AddHours(9).AddMinutes(10), Modes[1], (eau, 2));
        AjouterVente(today.AddHours(9).AddMinutes(57), Modes[0], (eau, 1));
        AjouterVente(today.AddHours(10).AddMinutes(30), Modes[0], (pain, 1));
        AjouterVente(today.AddHours(11).AddMinutes(5), Modes[2], (eau, 2), (jusF, 1));
        AjouterVente(today.AddHours(11).AddMinutes(43), Modes[0], (jusO, 1));
        AjouterVente(today.AddHours(12).AddMinutes(20), Modes[1], (jusO, 2), (eau, 2));
        AjouterVente(today.AddHours(13), Modes[0], (pain, 3));
        AjouterVente(today.AddHours(13).AddMinutes(40), Modes[0], (biscuits, 2));
        AjouterVente(today.AddHours(14).AddMinutes(12), Modes[0], (biscuits, 1));
        AjouterVente(today.AddHours(14).AddMinutes(50), Modes[2], (biscuits, 2), (jusO, 2));
    }
}
