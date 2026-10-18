namespace VenteJour.Models;

public class CategorieAide
{
    public string Id { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Clé d'icône : box, chart, cart, person_fill.</summary>
    public string Icone { get; set; } = "box";
}

public class ArticleAide
{
    public string Id { get; set; } = string.Empty;
    public string CategorieId { get; set; } = string.Empty;
    public string CategorieTitre { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Resume { get; set; } = string.Empty;
    public List<string> Paragraphes { get; set; } = new();
    public List<string> MotsCles { get; set; } = new();
}
