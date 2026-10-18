using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class AjouterProduitPage : ContentPage
{
    private readonly AjouterProduitViewModel _vm;

    public AjouterProduitPage(AjouterProduitViewModel vm)
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
