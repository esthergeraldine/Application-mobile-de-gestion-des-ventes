using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class AideCategoriePage : ContentPage
{
    private readonly AideCategorieViewModel _vm;

    public AideCategoriePage(AideCategorieViewModel vm)
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
