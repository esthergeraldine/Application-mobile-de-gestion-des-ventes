namespace VenteJour.Models;

public class LigneVente
{
    public int ProduitId { get; set; }
    public string NomProduit { get; set; } = string.Empty;
    public string Icone { get; set; } = "drop";
    public int Quantite { get; set; }
    public int PrixAchatUnitaire { get; set; }
    public int PrixVenteUnitaire { get; set; }

    public int Total => Quantite * PrixVenteUnitaire;
    public int Cout => Quantite * PrixAchatUnitaire;
    public int Benefice => Total - Cout;
    public string Libelle => $"{NomProduit} x {Quantite}";
}
