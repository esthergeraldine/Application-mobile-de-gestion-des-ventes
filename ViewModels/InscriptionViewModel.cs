using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class InscriptionViewModel : BaseViewModel
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;

    public InscriptionViewModel(IAuthService auth, INavigationService nav, IDialogService dialog)
    {
        _auth = auth;
        _nav = nav;
        _dialog = dialog;
    }

    [ObservableProperty] private string identifiant;
    [ObservableProperty] private string nomKiosque;
    [ObservableProperty] private string telephone;
    [ObservableProperty] private string motDePasse;
    [ObservableProperty] private string confirmation;

    [RelayCommand]
    private async Task SInscrireAsync()
    {
        if (IsBusy) return;
        MessageErreur = null;

        // Contrôles de saisie avec un message précis pour chaque cas
        if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(NomKiosque) ||
            string.IsNullOrWhiteSpace(Telephone) || string.IsNullOrEmpty(MotDePasse))
        {
            MessageErreur = "Veuillez remplir tous les champs.";
            return;
        }
        if (MotDePasse != Confirmation)
        {
            MessageErreur = "Les mots de passe ne correspondent pas.";
            return;
        }

        IsBusy = true;
        try
        {
            await _auth.InscriptionAsync(Identifiant, NomKiosque, Telephone, MotDePasse);
            MotDePasse = null;
            Confirmation = null;
            await _nav.GoToAsync(Routes.Racine(Routes.Accueil));
        }
        catch (AuthentificationException ex)
        {
            MessageErreur = ex.Message; // les champs saisis sont conservés
        }
        catch (Exception ex)
        {
            MessageErreur = "Inscription impossible : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Reinitialiser()
    {
        MotDePasse = null;
        Confirmation = null;
        MessageErreur = null;
    }

    [RelayCommand]
    private Task SeConnecterAsync() => _nav.GoToAsync(Routes.Racine(Routes.Connexion));

    [RelayCommand]
    private Task AfficherLegalAsync()
        => _dialog.AlertAsync("Informations légales", "Conditions d'utilisation et politique de confidentialité à venir.");
}
