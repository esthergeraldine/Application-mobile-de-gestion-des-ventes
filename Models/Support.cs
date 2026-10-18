namespace VenteJour.Models;

public class DemandeSupport
{
    public string Id { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Sujet { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
}

public class SignalementProbleme
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateProbleme { get; set; }
    public string ReferenceTransaction { get; set; }
    public string PieceJointeNom { get; set; }
    public string PieceJointeChemin { get; set; }
    public DateTime DateCreation { get; set; }
}

public class FichierSelectionne
{
    public string Nom { get; set; } = string.Empty;
    public string Chemin { get; set; } = string.Empty;
}

/// <summary>
/// Résultat d'un envoi. <see cref="Transmis"/> vaut false tant que l'envoi n'a été que simulé localement :
/// l'interface doit alors l'indiquer clairement à l'utilisateur.
/// </summary>
public class ResultatEnvoi
{
    public string Reference { get; set; } = string.Empty;
    public bool Transmis { get; set; }
}
