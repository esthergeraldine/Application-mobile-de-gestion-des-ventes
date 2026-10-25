using VenteJour.Services;

namespace VenteJour.Views;

public partial class SplashPage : ContentPage
{
    private readonly IAuthService _auth;

    public SplashPage(IAuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(2200);

        // « Se souvenir de moi » : on rouvre directement le compte mémorisé, sinon écran de connexion
        Models.Utilisateur utilisateur = null;
        try
        {
            utilisateur = await _auth.RestaurerSessionAsync();
        }
        catch
        {
            // Si la session ne peut pas être restaurée, on demande simplement de se connecter.
        }

        await Shell.Current.GoToAsync(utilisateur != null ? "//accueil" : "//connexion");
    }
}
