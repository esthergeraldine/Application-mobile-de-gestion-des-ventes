using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class InscriptionPage : ContentPage
{
    public InscriptionPage(InscriptionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
