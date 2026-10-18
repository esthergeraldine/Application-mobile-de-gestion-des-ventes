using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class NouvelleVentePage : ContentPage
{
    private readonly NouvelleVenteViewModel _vm;

    public NouvelleVentePage(NouvelleVenteViewModel vm)
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
