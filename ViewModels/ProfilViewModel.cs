using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class ProfilViewModel : BaseViewModel
{
    private readonly IAuthService _auth;
    private readonly IKiosqueService _kiosques;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;

    public ProfilViewModel(IAuthService auth, IKiosqueService kiosques, INavigationService nav, IDialogService dialog)
    {
        _auth = auth;
        _kiosques = kiosques;
        _nav = nav;
        _dialog = dialog;
    }

    [ObservableProperty] private string nom;
    [ObservableProperty] private string nomKiosque;

    [RelayCommand]
    private async Task ChargerAsync()
    {
        var k = await _kiosques.GetKiosqueAsync();
        Nom = k.Responsable;
        NomKiosque = k.Nom;
    }

    [RelayCommand]
    private Task InfosKiosqueAsync() => _nav.GoToAsync(Routes.Kiosque);

    [RelayCommand]
    private Task ParametresAsync() => _nav.GoToAsync(Routes.Parametres);

    [RelayCommand]
    private Task AideAsync() => _nav.GoToAsync(Routes.Aide);

    [RelayCommand]
    private async Task DeconnexionAsync()
    {
        if (!await _dialog.ConfirmAsync("Déconnexion", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non"))
            return;

        await _auth.DeconnexionAsync();
        await _nav.GoToAsync(Routes.Racine(Routes.Connexion));
    }
}
