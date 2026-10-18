namespace VenteJour.Models;

public static class StatutVente
{
    public const string Payee = "Payée";
    public const string Annulee = "Annulée";
}

public class Vente
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string ModeReglement { get; set; } = "Espèces";
    public string Statut { get; set; } = StatutVente.Payee;
    public DateTime? DateAnnulation { get; set; }
    public List<LigneVente> Lignes { get; set; } = new();

    /// <summary>Une vente annulée n'est jamais comptabilisée (recettes, bénéfices, classements, bilans).</summary>
    public bool EstValide => Statut != StatutVente.Annulee;

    public int Total => Lignes.Sum(l => l.Total);
    public int Cout => Lignes.Sum(l => l.Cout);
    public int Benefice => Total - Cout;
    public double MargePourcentage => Total == 0 ? 0 : Benefice * 100.0 / Total;
    public int NombreArticles => Lignes.Sum(l => l.Quantite);

    public string Libelle => Lignes.Count == 0
        ? string.Empty
        : Lignes.Count == 1 ? Lignes[0].NomProduit : $"{Lignes[0].NomProduit} +{Lignes.Count - 1}";

    public string Icone => Lignes.FirstOrDefault()?.Icone ?? "drop";
}
