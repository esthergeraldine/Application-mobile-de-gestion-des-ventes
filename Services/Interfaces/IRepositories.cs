using VenteJour.Models;

namespace VenteJour.Services;

// Phase A : contrats d'accès aux données (équivalent de IDepenseRepository dans le TP).
// Les services métier ne connaissent QUE ces interfaces : JSON aujourd'hui (phase A),
// SQLite demain (phase C) → une seule ligne à changer dans MauiProgram.cs.

public interface IProduitRepository
{
    Task<List<Produit>> ListerAsync();
    Task<Produit> ObtenirAsync(int id);

    /// <summary>Ajoute le produit et lui attribue un identifiant unique (retourné dans le produit).</summary>
    Task<Produit> AjouterAsync(Produit produit);

    Task MettreAJourAsync(Produit produit);
    Task RemplacerToutAsync(List<Produit> produits);
}

public interface IVenteRepository
{
    Task<List<Vente>> ListerAsync();
    Task<Vente> ObtenirAsync(string id);

    /// <summary>Ajoute la vente et lui attribue un identifiant unique (TRX001, TRX002...).</summary>
    Task<Vente> AjouterAsync(Vente vente);

    Task MettreAJourAsync(Vente vente);
    Task RemplacerToutAsync(List<Vente> ventes);
}

public interface IKiosqueRepository
{
    Task<Kiosque> ObtenirAsync();
    Task EnregistrerAsync(Kiosque kiosque);
}
