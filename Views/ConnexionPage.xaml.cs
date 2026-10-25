using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class ConnexionPage : ContentPage
{
    private readonly ConnexionViewModel _vm;

    public ConnexionPage(ConnexionViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.ReinitialiserCommand.Execute(null); // pas de mot de passe ni d'erreur résiduels (ex. après déconnexion)
    }
}
