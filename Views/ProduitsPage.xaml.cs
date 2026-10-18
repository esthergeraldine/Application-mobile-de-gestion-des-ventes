using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class ProduitsPage : ContentPage
{
    private readonly ProduitsViewModel _vm;

    public ProduitsPage(ProduitsViewModel vm)
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
