using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class ConnexionPage : ContentPage
{
    public ConnexionPage(ConnexionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
