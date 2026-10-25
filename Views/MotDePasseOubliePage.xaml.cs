using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class MotDePasseOubliePage : ContentPage
{
    public MotDePasseOubliePage(MotDePasseOublieViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
