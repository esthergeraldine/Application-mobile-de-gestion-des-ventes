using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class ContactSupportPage : ContentPage
{
    private readonly ContactSupportViewModel _vm;

    public ContactSupportPage(ContactSupportViewModel vm)
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
