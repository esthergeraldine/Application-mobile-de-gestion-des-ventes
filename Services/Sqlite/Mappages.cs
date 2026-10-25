using VenteJour.Models;

namespace VenteJour.Services.Sqlite;

internal static class Mappages
{
    public static ProduitEntite VersEntite(this Produit p) => new()
    {
        Id = p.Id, Nom = p.Nom, Categorie = p.Categorie, PrixAchat = p.PrixAchat,
        PrixVente = p.PrixVente, Stock = p.Stock, Icone = p.Icone
    };

    public static Produit VersModele(this ProduitEntite e) => new()
    {
        Id = e.Id, Nom = e.Nom, Categorie = e.Categorie, PrixAchat = e.PrixAchat,
        PrixVente = e.PrixVente, Stock = e.Stock, Icone = e.Icone
    };

    public static VenteEntite VersEntite(this Vente v) => new()
    {
        Id = v.Id, Date = v.Date, ModeReglement = v.ModeReglement, Statut = v.Statut, DateAnnulation = v.DateAnnulation
    };

    public static List<LigneVenteEntite> LignesVersEntites(this Vente v) => v.Lignes.Select(l => new LigneVenteEntite
    {
        VenteId = v.Id, ProduitId = l.ProduitId, NomProduit = l.NomProduit, Icone = l.Icone, Quantite = l.Quantite,
        PrixAchatUnitaire = l.PrixAchatUnitaire, PrixVenteUnitaire = l.PrixVenteUnitaire
    }).ToList();

    public static Vente VersModele(this VenteEntite e, IEnumerable<LigneVenteEntite> lignes) => new()
    {
        Id = e.Id, Date = e.Date, ModeReglement = e.ModeReglement, Statut = e.Statut, DateAnnulation = e.DateAnnulation,
        Lignes = lignes.OrderBy(l => l.Id).Select(l => new LigneVente
        {
            ProduitId = l.ProduitId, NomProduit = l.NomProduit, Icone = l.Icone, Quantite = l.Quantite,
            PrixAchatUnitaire = l.PrixAchatUnitaire, PrixVenteUnitaire = l.PrixVenteUnitaire
        }).ToList()
    };

    public static KiosqueEntite VersEntite(this Kiosque k) => new()
    {
        Id = 1, Nom = k.Nom, Responsable = k.Responsable, Telephone = k.Telephone,
        Region = k.Region, Ville = k.Ville, Quartier = k.Quartier, AdressePrecise = k.AdressePrecise
    };

    public static Kiosque VersModele(this KiosqueEntite e) => new()
    {
        Id = e.Id, Nom = e.Nom, Responsable = e.Responsable, Telephone = e.Telephone,
        Region = e.Region, Ville = e.Ville, Quartier = e.Quartier, AdressePrecise = e.AdressePrecise
    };
}
