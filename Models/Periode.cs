namespace VenteJour.Models;

public enum PeriodeVente
{
    Aujourdhui,
    Hier,
    Semaine
}

public class HistoriqueVentes
{
    public PeriodeVente Periode { get; set; }

    /// <summary>Total des ventes VALIDES (les ventes annulées sont exclues).</summary>
    public int Total { get; set; }

    /// <summary>Variation en % par rapport à la période précédente (null si pas de référence).</summary>
    public double? VariationPourcentage { get; set; }

    /// <summary>Toutes les ventes de la période, y compris annulées (identifiables par Statut).</summary>
    public List<Vente> Ventes { get; set; } = new();
}

public enum PeriodeClassement
{
    Aujourdhui,
    Semaine,
    Mois,
    Personnalisee
}

public enum CritereClassement
{
    Recettes,
    Benefices
}

/// <summary>Intervalle [Debut, Fin[ (Fin exclusive).</summary>
public class PlageDates
{
    public PlageDates(DateTime debut, DateTime fin)
    {
        Debut = debut;
        Fin = fin;
    }

    public DateTime Debut { get; }
    public DateTime Fin { get; }

    public bool Contient(DateTime date) => date >= Debut && date < Fin;

    /// <summary>Plages de l'écran Historique : Aujourd'hui, Hier, ou les 7 derniers jours (aujourd'hui inclus).</summary>
    public static PlageDates Pour(PeriodeVente periode)
    {
        var aujourdhui = DateTime.Today;
        return periode switch
        {
            PeriodeVente.Hier => new PlageDates(aujourdhui.AddDays(-1), aujourdhui),
            PeriodeVente.Semaine => new PlageDates(aujourdhui.AddDays(-6), aujourdhui.AddDays(1)),
            _ => new PlageDates(aujourdhui, aujourdhui.AddDays(1))
        };
    }

    /// <summary>
    /// Aujourd'hui = 00:00 → lendemain 00:00. Cette semaine = du lundi au dimanche. Ce mois = du 1er au dernier jour.
    /// Personnalisée = du jour de début inclus au jour de fin inclus.
    /// </summary>
    public static PlageDates Pour(PeriodeClassement periode, DateTime maintenant, DateTime? debut = null, DateTime? fin = null)
    {
        var jour = maintenant.Date;
        switch (periode)
        {
            case PeriodeClassement.Semaine:
                int ecart = ((int)jour.DayOfWeek + 6) % 7; // lundi = 0
                var lundi = jour.AddDays(-ecart);
                return new PlageDates(lundi, lundi.AddDays(7));
            case PeriodeClassement.Mois:
                var premier = new DateTime(jour.Year, jour.Month, 1);
                return new PlageDates(premier, premier.AddMonths(1));
            case PeriodeClassement.Personnalisee:
                var d = (debut ?? jour).Date;
                var f = (fin ?? jour).Date;
                if (f < d) (d, f) = (f, d);
                return new PlageDates(d, f.AddDays(1));
            default:
                return new PlageDates(jour, jour.AddDays(1));
        }
    }
}
