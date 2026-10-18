using VenteJour.Models;

namespace VenteJour.Services;

public interface IVenteService
{
    Task<HistoriqueVentes> GetHistoriqueAsync(PeriodeVente periode);
    Task<Vente> GetVenteAsync(string id);

    /// <summary>Enregistre une vente et décrémente le stock. Lève InvalidOperationException si stock insuffisant.</summary>
    Task<Vente> EnregistrerVenteAsync(IEnumerable<LigneVente> lignes, string modeReglement);

    /// <summary>Annule une vente (définitif) : remet les articles en stock et exclut la vente de tous les calculs.</summary>
    Task<Vente> AnnulerVenteAsync(string id);
}
