using VenteJour.Models;

namespace VenteJour.Domain;

/// <summary>
/// Règles de calcul centralisées, sans dépendance à l'interface ni au stockage :
/// réutilisables par les services fictifs, par les futurs services HTTP et par des tests unitaires.
///
/// Recette d'une ligne  = quantité × prix de vente unitaire (figé à la vente)
/// Coût d'une ligne     = quantité × prix d'achat unitaire (figé à la vente => un changement de prix
///                        ultérieur ne modifie jamais le bénéfice des anciennes ventes)
/// Bénéfice d'une ligne = recette - coût (négatif pour une vente à perte)
/// Seules les ventes valides (non annulées) sont comptabilisées.
/// </summary>
public static class StatistiquesCalculator
{
    public static IEnumerable<Vente> VentesValides(IEnumerable<Vente> ventes, PlageDates plage)
        => ventes.Where(v => v.EstValide && plage.Contient(v.Date));

    public static Bilan CalculerBilan(IEnumerable<Vente> ventes, DateTime jour)
    {
        var plage = PlageDates.Pour(PeriodeClassement.Aujourdhui, jour);
        var valides = VentesValides(ventes, plage).ToList();
        int recette = valides.Sum(v => v.Total);
        int benefice = valides.Sum(v => v.Benefice);

        return new Bilan
        {
            Jour = jour.Date,
            Recette = recette,
            Benefice = benefice,
            MargePourcentage = recette == 0 ? 0 : benefice * 100.0 / recette,
            NombreTransactions = valides.Count,
            ArticlesSortis = valides.Sum(v => v.NombreArticles)
        };
    }

    public static int TotalRecette(IEnumerable<Vente> ventes, PlageDates plage)
        => VentesValides(ventes, plage).Sum(v => v.Total);

    /// <summary>
    /// Classement des produits ayant des ventes valides sur la période (les produits sans vente n'apparaissent pas).
    /// Tri déterministe :
    ///  - Recettes  : recette ↓, quantité ↓, bénéfice ↓, nom ↑, id ↑
    ///  - Bénéfices : bénéfice ↓, recette ↓, quantité ↓, nom ↑, id ↑
    /// </summary>
    public static IReadOnlyList<ClassementProduit> Classer(
        IEnumerable<Vente> ventes, IEnumerable<Produit> produits, PlageDates plage,
        CritereClassement critere, int? limite = null)
    {
        var catalogue = produits.ToDictionary(p => p.Id);

        var agregats = VentesValides(ventes, plage)
            .SelectMany(v => v.Lignes)
            .GroupBy(l => l.ProduitId)
            .Select(g =>
            {
                catalogue.TryGetValue(g.Key, out var produit);
                var derniere = g.Last();
                return new ClassementProduit
                {
                    ProduitId = g.Key,
                    Nom = produit?.Nom ?? derniere.NomProduit,
                    Icone = produit?.Icone ?? derniere.Icone,
                    PrixVente = produit?.PrixVente ?? derniere.PrixVenteUnitaire,
                    QuantiteVendue = g.Sum(l => l.Quantite),
                    Recette = g.Sum(l => l.Total),
                    Benefice = g.Sum(l => l.Benefice)
                };
            })
            .Where(a => a.QuantiteVendue > 0)
            .ToList();

        IOrderedEnumerable<ClassementProduit> tri = critere == CritereClassement.Recettes
            ? agregats.OrderByDescending(a => a.Recette).ThenByDescending(a => a.QuantiteVendue).ThenByDescending(a => a.Benefice)
            : agregats.OrderByDescending(a => a.Benefice).ThenByDescending(a => a.Recette).ThenByDescending(a => a.QuantiteVendue);

        var resultat = tri.ThenBy(a => a.Nom, StringComparer.OrdinalIgnoreCase).ThenBy(a => a.ProduitId).ToList();
        if (limite.HasValue)
            resultat = resultat.Take(limite.Value).ToList();

        int meilleur = resultat.Count == 0 ? 0 : resultat.Max(a => Valeur(a, critere));
        for (int i = 0; i < resultat.Count; i++)
        {
            var a = resultat[i];
            a.Rang = i + 1;
            a.Valeur = Valeur(a, critere);
            a.PartRelative = meilleur <= 0 || a.Valeur <= 0 ? 0 : Math.Min(1.0, (double)a.Valeur / meilleur);
        }
        return resultat;
    }

    private static int Valeur(ClassementProduit a, CritereClassement critere)
        => critere == CritereClassement.Recettes ? a.Recette : a.Benefice;
}
