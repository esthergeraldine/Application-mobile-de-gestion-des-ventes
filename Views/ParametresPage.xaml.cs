using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class ParametresPage : ContentPage
{
    public ParametresPage(ParametresViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
