using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Models;
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
        MessageErreur = null;
        if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(NomKiosque) ||
            string.IsNullOrWhiteSpace(Telephone) || string.IsNullOrWhiteSpace(MotDePasse))
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
            await _auth.InscriptionAsync(new Utilisateur
            {
                Email = Identifiant, NomKiosque = NomKiosque, Telephone = Telephone
            }, MotDePasse);
            await _nav.GoToAsync(Routes.Racine(Routes.Accueil));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task SeConnecterAsync() => _nav.GoToAsync(Routes.Racine(Routes.Connexion));

    [RelayCommand]
    private Task AfficherLegalAsync()
        => _dialog.AlertAsync("Informations légales", "Conditions d'utilisation et politique de confidentialité à venir.");
}
