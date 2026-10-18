using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class AideArticleViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAideService _aide;
    private string _id;

    public AideArticleViewModel(IAideService aide) => _aide = aide;

    [ObservableProperty] private ArticleAide article;

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
            Article = await _aide.GetArticleAsync(_id);
            if (Article == null)
                MessageErreur = "Cet article est introuvable.";
        }
        catch (Exception ex)
        {
            MessageErreur = "Impossible de charger l'article : " + ex.Message;
        }
    }
}
