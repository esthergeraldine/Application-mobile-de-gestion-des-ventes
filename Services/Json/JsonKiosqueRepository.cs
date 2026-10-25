using VenteJour.Models;

namespace VenteJour.Services.Json;

/// <summary>Phase A : informations du kiosque stockées dans kiosque.json.</summary>
public class JsonKiosqueRepository : IKiosqueRepository
{
    private readonly JsonFichier<Kiosque> _fichier;

    public JsonKiosqueRepository(string dossier, Func<Kiosque> donneesInitiales = null)
        => _fichier = new JsonFichier<Kiosque>(dossier, "kiosque.json", donneesInitiales);

    public Task<Kiosque> ObtenirAsync() => _fichier.LireAsync();

    public Task EnregistrerAsync(Kiosque kiosque) => _fichier.RemplacerAsync(kiosque);
}
