namespace VenteJour.Models;

/// <summary>
/// Compte enregistré sur le téléphone. Le mot de passe n'est JAMAIS stocké : seulement son empreinte PBKDF2
/// (hash) et le sel aléatoire utilisé pour la calculer.
/// </summary>
public class CompteLocal
{
    public int Id { get; set; }

    /// <summary>E-mail (en minuscules) ou numéro à 9 chiffres saisi à l'inscription.</summary>
    public string Identifiant { get; set; } = string.Empty;

    /// <summary>Téléphone du kiosque, 9 chiffres.</summary>
    public string Telephone { get; set; } = string.Empty;

    public string NomKiosque { get; set; } = string.Empty;
    public string Sel { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public int Iterations { get; set; }
    public DateTime DateCreation { get; set; }
}
