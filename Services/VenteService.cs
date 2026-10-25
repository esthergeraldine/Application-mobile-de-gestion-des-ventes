using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>
/// Phase A : règles de vente (stock, annulation, historique) au-dessus des dépôts de données.
/// Un verrou garantit que deux ventes simultanées ne peuvent pas vendre le même stock deux fois.
/// </summary>
public class VenteService : IVenteService
{
    private static readonly SemaphoreSlim Verrou = new(1, 1);

    private readonly IVenteRepository _ventes;
    private readonly IProduitRepository _produits;

    public VenteService(IVenteRepository ventes, IProduitRepository produits)
    {
        _ventes = ventes;
        _produits = produits;
    }

    public async Task<HistoriqueVentes> GetHistoriqueAsync(PeriodeVente periode)
    {
        var toutes = await _ventes.ListerAsync();
        var plage = PlageDates.Pour(periode);
        var duree = plage.Fin - plage.Debut;
        var precedente = new PlageDates(plage.Debut - duree, plage.Debut);

        int total = StatistiquesCalculator.TotalRecette(toutes, plage); // ventes valides uniquement
        int avant = StatistiquesCalculator.TotalRecette(toutes, precedente);

        return new HistoriqueVentes
        {
            Periode = periode,
            Total = total,
            VariationPourcentage = avant == 0 ? null : (total - avant) * 100.0 / avant,
            Ventes = toutes.Where(v => plage.Contient(v.Date)).OrderByDescending(v => v.Date).ToList()
        };
    }

    public Task<Vente> GetVenteAsync(string id) => _ventes.ObtenirAsync(id);

    public async Task<Vente> EnregistrerVenteAsync(IEnumerable<LigneVente> lignes, string modeReglement)
    {
        var liste = lignes.ToList();
        if (liste.Count == 0)
            throw new InvalidOperationException("La vente ne contient aucun article.");

        await Verrou.WaitAsync();
        try
        {
            var catalogue = (await _produits.ListerAsync()).ToDictionary(p => p.Id);

            // Contrôle du stock produit par produit (plusieurs lignes du même produit sont additionnées)
            foreach (var groupe in liste.GroupBy(l => l.ProduitId))
            {
                if (!catalogue.TryGetValue(groupe.Key, out var produit))
                    throw new InvalidOperationException("Produit introuvable.");
                if (groupe.Sum(l => l.Quantite) > produit.Stock)
                    throw new InvalidOperationException($"Stock insuffisant pour {produit.Nom} (disponible : {produit.Stock}).");
            }

            var stockAvant = catalogue.Values.ToDictionary(p => p.Id, p => p.Stock);
            var modifies = new List<Produit>();
            foreach (var groupe in liste.GroupBy(l => l.ProduitId))
            {
                var produit = catalogue[groupe.Key];
                produit.Stock -= groupe.Sum(l => l.Quantite);
                await _produits.MettreAJourAsync(produit);
                modifies.Add(produit);
            }

            try
            {
                return await _ventes.AjouterAsync(new Vente
                {
                    Date = DateTime.Now, ModeReglement = modeReglement, Lignes = liste
                });
            }
            catch
            {
                // La vente n'a pas pu être écrite : on remet le stock comme avant pour ne pas le fausser.
                foreach (var produit in modifies)
                {
                    produit.Stock = stockAvant[produit.Id];
                    await _produits.MettreAJourAsync(produit);
                }
                throw;
            }
        }
        finally
        {
            Verrou.Release();
        }
    }

    public async Task<Vente> AnnulerVenteAsync(string id)
    {
        await Verrou.WaitAsync();
        try
        {
            var vente = await _ventes.ObtenirAsync(id) ?? throw new InvalidOperationException("Transaction introuvable.");
            if (!vente.EstValide)
                throw new InvalidOperationException("Cette vente est déjà annulée.");

            var catalogue = (await _produits.ListerAsync()).ToDictionary(p => p.Id);
            var stockAvant = new Dictionary<int, int>();
            foreach (var ligne in vente.Lignes)
            {
                if (!catalogue.TryGetValue(ligne.ProduitId, out var produit))
                    continue; // produit disparu du catalogue : rien à remettre en stock
                if (!stockAvant.ContainsKey(produit.Id))
                    stockAvant[produit.Id] = produit.Stock;
                produit.Stock += ligne.Quantite; // les articles reviennent en stock
            }
            foreach (var produitId in stockAvant.Keys)
                await _produits.MettreAJourAsync(catalogue[produitId]);

            vente.Statut = StatutVente.Annulee;
            vente.DateAnnulation = DateTime.Now;
            try
            {
                await _ventes.MettreAJourAsync(vente);
            }
            catch
            {
                foreach (var (produitId, stock) in stockAvant)
                {
                    catalogue[produitId].Stock = stock;
                    await _produits.MettreAJourAsync(catalogue[produitId]);
                }
                throw;
            }
            return vente;
        }
        finally
        {
            Verrou.Release();
        }
    }
}
