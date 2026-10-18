using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class HistoriquePage : ContentPage
{
    private readonly HistoriqueViewModel _vm;

    public HistoriquePage(HistoriqueViewModel vm)
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
