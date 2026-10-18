using VenteJour.Services;

namespace VenteJour;

public partial class App : Application
{
    public App(IParametresService parametres)
    {
        InitializeComponent();
        parametres.AppliquerTheme();
    }

    protected override Window CreateWindow(IActivationState activationState)
        => new Window(new AppShell());
}
