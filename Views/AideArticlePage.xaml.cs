using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class AideArticlePage : ContentPage
{
    private readonly AideArticleViewModel _vm;

    public AideArticlePage(AideArticleViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.ChargerCommand.ExecuteAsync(null);
    }
}
