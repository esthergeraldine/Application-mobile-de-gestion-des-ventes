using VenteJour.Domain;
using VenteJour.Models;

namespace VenteJour.Services.Fake;

public class FakeAideService : IAideService
{
    public Task<IReadOnlyList<CategorieAide>> GetCategoriesAsync()
        => Task.FromResult<IReadOnlyList<CategorieAide>>(FakeAideContenu.Categories.ToList());

    public Task<CategorieAide> GetCategorieAsync(string id)
        => Task.FromResult(FakeAideContenu.Categories.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<ArticleAide>> GetArticlesAsync(string categorieId)
        => Task.FromResult<IReadOnlyList<ArticleAide>>(
            FakeAideContenu.Articles.Where(a => a.CategorieId == categorieId).ToList());

    public Task<ArticleAide> GetArticleAsync(string id)
        => Task.FromResult(FakeAideContenu.Articles.FirstOrDefault(a => a.Id == id));

    public Task<IReadOnlyList<ArticleAide>> RechercherAsync(string texte)
        => Task.FromResult(RechercheAide.Filtrer(FakeAideContenu.Articles, texte));
}
