using SQLite;

namespace VenteJour.Services.Sqlite;

// Phase C : tables SQLite. Ces classes ne servent qu'au stockage ; l'application continue de travailler
// avec les modèles (Produit, Vente...) : les dépôts convertissent dans les deux sens.

[Table("Produits")]
public class ProduitEntite
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nom { get; set; }
    public string Categorie { get; set; }
    public int PrixAchat { get; set; }
    public int PrixVente { get; set; }
    public int Stock { get; set; }
    public string Icone { get; set; }
}

[Table("Ventes")]
public class VenteEntite
{
    [PrimaryKey]
    public string Id { get; set; }

    [Indexed]
    public DateTime Date { get; set; }

    public string ModeReglement { get; set; }
    public string Statut { get; set; }
    public DateTime? DateAnnulation { get; set; }
}

[Table("LignesVente")]
public class LigneVenteEntite
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string VenteId { get; set; }

    public int ProduitId { get; set; }
    public string NomProduit { get; set; }
    public string Icone { get; set; }
    public int Quantite { get; set; }

    // Prix figés au moment de la vente : modifier le produit ensuite ne change pas les anciens bénéfices
    public int PrixAchatUnitaire { get; set; }
    public int PrixVenteUnitaire { get; set; }
}

[Table("Kiosque")]
public class KiosqueEntite
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Nom { get; set; }
    public string Responsable { get; set; }
    public string Telephone { get; set; }
    public string Region { get; set; }
    public string Ville { get; set; }
    public string Quartier { get; set; }
    public string AdressePrecise { get; set; }
}
