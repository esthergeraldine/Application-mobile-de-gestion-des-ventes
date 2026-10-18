using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class ProfilPage : ContentPage
{
    private readonly ProfilViewModel _vm;

    public ProfilPage(ProfilViewModel vm)
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
