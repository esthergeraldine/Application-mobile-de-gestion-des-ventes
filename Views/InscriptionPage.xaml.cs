using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class InscriptionPage : ContentPage
{
    private readonly InscriptionViewModel _vm;

    public InscriptionPage(InscriptionViewModel vm)
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
