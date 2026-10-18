using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class AideCategorieViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAideService _aide;
    private readonly INavigationService _nav;
    private string _id;

    public AideCategorieViewModel(IAideService aide, INavigationService nav)
    {
        _aide = aide;
        _nav = nav;
    }

    public ObservableCollection<ArticleAide> Articles { get; } = new();

    [ObservableProperty] private string titre = "Aide";
    [ObservableProperty] private string description;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var id) && id != null)
            _id = Convert.ToString(id, CultureInfo.InvariantCulture);
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        MessageErreur = null;
        try
        {
            var categorie = await _aide.GetCategorieAsync(_id);
            if (categorie == null)
            {
                MessageErreur = "Cette catégorie d'aide est introuvable.";
                return;
            }
            Titre = categorie.Titre;
            Description = categorie.Description;

            Articles.Clear();
            foreach (var a in await _aide.GetArticlesAsync(_id))
                Articles.Add(a);
        }
        catch (Exception ex)
        {
            MessageErreur = "Impossible de charger les articles : " + ex.Message;
        }
    }

    [RelayCommand]
    private Task OuvrirArticleAsync(ArticleAide article)
        => article == null ? Task.CompletedTask : _nav.GoToAsync($"{Routes.AideArticle}?id={article.Id}");
}
