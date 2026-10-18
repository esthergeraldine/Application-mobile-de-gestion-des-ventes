using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class AccueilPage : ContentPage
{
    private readonly AccueilViewModel _vm;

    public AccueilPage(AccueilViewModel vm)
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
