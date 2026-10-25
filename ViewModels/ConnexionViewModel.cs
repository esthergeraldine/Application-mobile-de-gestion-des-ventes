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
        if (IsBusy) return;
        MessageErreur = null;
        if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrEmpty(MotDePasse))
        {
            MessageErreur = "Veuillez saisir votre e-mail ou téléphone et votre mot de passe.";
            return;
        }

        IsBusy = true;
        try
        {
            await _auth.ConnexionAsync(Identifiant, MotDePasse, Souvenir);
            MotDePasse = null; // le mot de passe ne reste pas en mémoire dans le formulaire
            await _nav.GoToAsync(Routes.Racine(Routes.Accueil));
        }
        catch (AuthentificationException ex)
        {
            MessageErreur = ex.Message;
        }
        catch (Exception ex)
        {
            MessageErreur = "Connexion impossible : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Appelé à chaque affichage de l'écran : efface le mot de passe et les messages (ex. après une déconnexion).</summary>
    [RelayCommand]
    private void Reinitialiser()
    {
        MotDePasse = null;
        MessageErreur = null;
    }

    [RelayCommand]
    private Task CreerCompteAsync() => _nav.GoToAsync(Routes.Racine(Routes.Inscription));

    [RelayCommand]
    private Task MotDePasseOublieAsync()
        => _dialog.AlertAsync("Mot de passe oublié", "La réinitialisation sera disponible avec la future API.");
}
