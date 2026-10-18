using VenteJour.Models;

namespace VenteJour.Services;

public interface IAideService
{
    Task<IReadOnlyList<CategorieAide>> GetCategoriesAsync();
    Task<CategorieAide> GetCategorieAsync(string id);
    Task<IReadOnlyList<ArticleAide>> GetArticlesAsync(string categorieId);
    Task<ArticleAide> GetArticleAsync(string id);

    /// <summary>Recherche insensible à la casse et aux accents (titre, catégorie, contenu).</summary>
    Task<IReadOnlyList<ArticleAide>> RechercherAsync(string texte);
}
