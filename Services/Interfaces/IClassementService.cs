using VenteJour.Models;

namespace VenteJour.Services;

/// <summary>
/// Classement des produits calculé à partir des ventes valides enregistrées.
/// Les règles de calcul sont dans Domain/StatistiquesCalculator (réutilisables côté API).
/// </summary>
public interface IClassementService
{
    Task<IReadOnlyList<ClassementProduit>> GetClassementAsync(CritereClassement critere, PlageDates plage, int? limite = null);
}
