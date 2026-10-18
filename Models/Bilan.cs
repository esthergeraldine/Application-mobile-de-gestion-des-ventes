namespace VenteJour.Models;

public class Bilan
{
    public DateTime Jour { get; set; }
    public int Recette { get; set; }
    public int Benefice { get; set; }
    public double MargePourcentage { get; set; }
    public int NombreTransactions { get; set; }
    public int ArticlesSortis { get; set; }
}

/// <summary>Ligne d'un classement : agrégat des ventes valides d'un produit sur une période.</summary>
public class ClassementProduit
{
    public int Rang { get; set; }
    public int ProduitId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public int PrixVente { get; set; }
    public string Icone { get; set; } = "drop";
    public int QuantiteVendue { get; set; }
    public int Recette { get; set; }
    public int Benefice { get; set; }

    /// <summary>Valeur du critère de classement choisi (recette ou bénéfice).</summary>
    public int Valeur { get; set; }

    /// <summary>Part (0 à 1) de la valeur par rapport au 1er du classement. 0 si la valeur est négative ou nulle.</summary>
    public double PartRelative { get; set; }
}
