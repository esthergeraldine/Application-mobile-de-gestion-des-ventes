using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services.Fake;

public class FakeVenteService : IVenteService
{
    private readonly FakeDataStore _store;
    public FakeVenteService(FakeDataStore store) => _store = store;

    public Task<HistoriqueVentes> GetHistoriqueAsync(PeriodeVente periode)
    {
        var plage = Plage(periode);
        var duree = plage.Fin - plage.Debut;
        var precedente = new PlageDates(plage.Debut - duree, plage.Debut);

        var ventes = _store.Ventes.Where(v => plage.Contient(v.Date)).OrderByDescending(v => v.Date).ToList();
        int total = StatistiquesCalculator.TotalRecette(_store.Ventes, plage); // ventes valides uniquement
        int avant = StatistiquesCalculator.TotalRecette(_store.Ventes, precedente);
        double? variation = avant == 0 ? null : (total - avant) * 100.0 / avant;

        return Task.FromResult(new HistoriqueVentes
        {
            Periode = periode, Total = total, VariationPourcentage = variation, Ventes = ventes
        });
    }

    public Task<Vente> GetVenteAsync(string id)
        => Task.FromResult(_store.Ventes.FirstOrDefault(v => v.Id == id));

    public Task<Vente> EnregistrerVenteAsync(IEnumerable<LigneVente> lignes, string modeReglement)
    {
        var liste = lignes.ToList();
        foreach (var l in liste)
        {
            var p = _store.Produits.FirstOrDefault(x => x.Id == l.ProduitId);
            if (p == null)
                throw new InvalidOperationException("Produit introuvable.");
            if (l.Quantite > p.Stock)
                throw new InvalidOperationException($"Stock insuffisant pour {p.Nom} (disponible : {p.Stock}).");
        }

        foreach (var l in liste)
            _store.Produits.First(x => x.Id == l.ProduitId).Stock -= l.Quantite;

        var vente = new Vente
        {
            Id = _store.NouvelIdVente(), Date = DateTime.Now,
            ModeReglement = modeReglement, Lignes = liste
        };
        _store.Ventes.Add(vente);
        return Task.FromResult(vente);
    }

    public Task<Vente> AnnulerVenteAsync(string id)
    {
        var vente = _store.Ventes.FirstOrDefault(v => v.Id == id)
                    ?? throw new InvalidOperationException("Transaction introuvable.");
        if (!vente.EstValide)
            throw new InvalidOperationException("Cette vente est déjà annulée.");

        foreach (var l in vente.Lignes)
        {
            var p = _store.Produits.FirstOrDefault(x => x.Id == l.ProduitId);
            if (p != null)
                p.Stock += l.Quantite; // les articles reviennent en stock
        }

        vente.Statut = StatutVente.Annulee;
        vente.DateAnnulation = DateTime.Now;
        return Task.FromResult(vente);
    }

    internal static PlageDates Plage(PeriodeVente periode)
    {
        var today = DateTime.Today;
        return periode switch
        {
            PeriodeVente.Hier => new PlageDates(today.AddDays(-1), today),
            PeriodeVente.Semaine => new PlageDates(today.AddDays(-6), today.AddDays(1)),
            _ => new PlageDates(today, today.AddDays(1))
        };
    }
}
