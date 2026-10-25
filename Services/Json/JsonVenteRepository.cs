using VenteJour.Models;

namespace VenteJour.Services.Json;

/// <summary>
/// Phase A : ventes stockées dans ventes.json. Chaque ligne de vente conserve le prix d'achat ET le prix de vente
/// du jour : modifier un produit plus tard ne change jamais le bénéfice des anciennes ventes.
/// </summary>
public class JsonVenteRepository : IVenteRepository
{
    private readonly JsonFichier<List<Vente>> _fichier;

    public JsonVenteRepository(string dossier, Func<List<Vente>> donneesInitiales = null)
        => _fichier = new JsonFichier<List<Vente>>(dossier, "ventes.json", donneesInitiales);

    public Task<List<Vente>> ListerAsync() => _fichier.LireAsync();

    public async Task<Vente> ObtenirAsync(string id)
        => (await _fichier.LireAsync()).FirstOrDefault(v => v.Id == id);

    public Task<Vente> AjouterAsync(Vente vente)
        => _fichier.TransformerAsync(liste =>
        {
            int dernier = liste.Select(v => Numero(v.Id)).DefaultIfEmpty(0).Max();
            vente.Id = $"TRX{dernier + 1:000}";
            liste.Add(vente);
            return vente;
        });

    public Task MettreAJourAsync(Vente vente)
        => _fichier.TransformerAsync(liste =>
        {
            var index = liste.FindIndex(v => v.Id == vente.Id);
            if (index < 0)
                throw new InvalidOperationException("Transaction introuvable.");
            liste[index] = vente;
            return true;
        });

    public Task RemplacerToutAsync(List<Vente> ventes) => _fichier.RemplacerAsync(ventes);

    private static int Numero(string id)
        => id != null && id.StartsWith("TRX") && int.TryParse(id.Substring(3), out var n) ? n : 0;
}
