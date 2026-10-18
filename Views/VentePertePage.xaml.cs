using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class VentePertePage : ContentPage
{
    private readonly VentePerteViewModel _vm;

    public VentePertePage(VentePerteViewModel vm)
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
