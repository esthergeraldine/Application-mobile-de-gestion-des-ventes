using VenteJour.Models;

namespace VenteJour.Services;

public class KiosqueService : IKiosqueService
{
    private readonly IKiosqueRepository _kiosque;
    public KiosqueService(IKiosqueRepository kiosque) => _kiosque = kiosque;

    /// <summary>True : les données sont enregistrées sur le téléphone mais aucun serveur n'est contacté.</summary>
    public bool EstSimulation => true;

    public Task<Kiosque> GetKiosqueAsync() => _kiosque.ObtenirAsync();

    public Task EnregistrerAsync(Kiosque kiosque) => _kiosque.EnregistrerAsync(kiosque);
}
