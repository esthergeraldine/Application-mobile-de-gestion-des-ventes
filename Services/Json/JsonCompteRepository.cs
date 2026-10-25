using VenteJour.Models;

namespace VenteJour.Services.Json;

/// <summary>Comptes stockés dans comptes.json (dossier privé de l'application). Contient des empreintes, jamais de mots de passe.</summary>
public class JsonCompteRepository : ICompteRepository
{
    private readonly JsonFichier<List<CompteLocal>> _fichier;

    public JsonCompteRepository(string dossier)
        => _fichier = new JsonFichier<List<CompteLocal>>(dossier, "comptes.json", null);

    public Task<List<CompteLocal>> ListerAsync() => _fichier.LireAsync();

    public Task<CompteLocal> AjouterAsync(CompteLocal compte)
        => _fichier.TransformerAsync(liste =>
        {
            compte.Id = (liste.Count == 0 ? 0 : liste.Max(c => c.Id)) + 1;
            liste.Add(compte);
            return compte;
        });

    public Task MettreAJourAsync(CompteLocal compte)
        => _fichier.TransformerAsync(liste =>
        {
            var index = liste.FindIndex(c => c.Id == compte.Id);
            if (index < 0)
                throw new InvalidOperationException("Compte introuvable.");
            liste[index] = compte;
            return true;
        });
}
