using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class AidePage : ContentPage
{
    private readonly AideViewModel _vm;

    public AidePage(AideViewModel vm)
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
