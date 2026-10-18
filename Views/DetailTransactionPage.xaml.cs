using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class DetailTransactionPage : ContentPage
{
    private readonly DetailTransactionViewModel _vm;

    public DetailTransactionPage(DetailTransactionViewModel vm)
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
