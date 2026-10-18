using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class ConnexionViewModel : BaseViewModel
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;

    public ConnexionViewModel(IAuthService auth, INavigationService nav, IDialogService dialog)
    {
        _auth = auth;
        _nav = nav;
        _dialog = dialog;
    }

    [ObservableProperty] private string identifiant;
    [ObservableProperty] private string motDePasse;
    [ObservableProperty] private bool souvenir = true;

    [RelayCommand]
    private async Task SeConnecterAsync()
    {
        MessageErreur = null;
        if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(MotDePasse))
        {
            MessageErreur = "Veuillez saisir votre e-mail ou téléphone et votre mot de passe.";
            return;
        }

        IsBusy = true;
        try
        {
            var utilisateur = await _auth.ConnexionAsync(Identifiant, MotDePasse);
            if (utilisateur == null)
            {
                MessageErreur = "Identifiants incorrects.";
                return;
            }
            await _nav.GoToAsync(Routes.Racine(Routes.Accueil));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task CreerCompteAsync() => _nav.GoToAsync(Routes.Racine(Routes.Inscription));

    [RelayCommand]
    private Task MotDePasseOublieAsync()
        => _dialog.AlertAsync("Mot de passe oublié", "La réinitialisation sera disponible avec la future API.");
}
