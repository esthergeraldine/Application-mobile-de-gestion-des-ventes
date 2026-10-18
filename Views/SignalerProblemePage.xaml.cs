using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class SignalerProblemePage : ContentPage
{
    private readonly SignalerProblemeViewModel _vm;

    public SignalerProblemePage(SignalerProblemeViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
}
