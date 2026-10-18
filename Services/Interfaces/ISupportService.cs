using VenteJour.Models;

namespace VenteJour.Services;

public interface ISupportService
{
    /// <summary>True tant que les demandes ne sont pas réellement transmises à un serveur.</summary>
    bool EstSimulation { get; }

    Task<ResultatEnvoi> EnvoyerDemandeAsync(DemandeSupport demande);
    Task<ResultatEnvoi> EnvoyerSignalementAsync(SignalementProbleme signalement);
}

public interface ISelecteurFichierService
{
    /// <summary>Ouvre le sélecteur d'images. Retourne null si l'utilisateur annule.</summary>
    Task<FichierSelectionne> ChoisirImageAsync();
}
