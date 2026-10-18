using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class ClassementPage : ContentPage
{
    private readonly ClassementViewModel _vm;

    public ClassementPage(ClassementViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // Recalcul à chaque affichage : le classement reflète toujours les ventes enregistrées entre-temps.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.ChargerCommand.ExecuteAsync(null);
    }
}
