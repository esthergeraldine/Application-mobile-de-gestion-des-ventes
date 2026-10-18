using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class SuccesPage : ContentPage
{
    public SuccesPage(SuccesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
