using VenteJour.Models;

namespace VenteJour.Services;

public interface IBilanService
{
    Task<Bilan> GetBilanAsync(DateTime jour);
}
