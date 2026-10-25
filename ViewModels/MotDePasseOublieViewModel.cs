using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class MotDePasseOublieViewModel : BaseViewModel
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;

    public MotDePasseOublieViewModel(IAuthService auth, INavigationService nav, IDialogService dialog)
    {
        _auth = auth;
        _nav = nav;
        _dialog = dialog;
    }

    [ObservableProperty] private string identifiant;
    [ObservableProperty] private string codeRecuperation;
    [ObservableProperty] private string nouveauMotDePasse;
    [ObservableProperty] private string confirmation;

    [RelayCommand]
    private async Task ReinitialiserAsync()
    {
        if (IsBusy) return;
        MessageErreur = null;

        if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(CodeRecuperation) ||
            string.IsNullOrEmpty(NouveauMotDePasse))
        {
            MessageErreur = "Veuillez remplir tous les champs.";
            return;
        }
        if (NouveauMotDePasse != Confirmation)
        {
            MessageErreur = "Les mots de passe ne correspondent pas.";
            return;
        }

        IsBusy = true;
        try
        {
            var nouveauCode = await _auth.ReinitialiserMotDePasseAsync(Identifiant, CodeRecuperation, NouveauMotDePasse);
            NouveauMotDePasse = null;
            Confirmation = null;

            // Un code ne sert qu'une fois : on remet immédiatement le nouveau.
            await _dialog.AlertAsync("Mot de passe modifié",
                $"Votre mot de passe a été changé. Voici votre NOUVEAU code de récupération (l'ancien n'est plus valable) :\n\n{nouveauCode}\n\n" +
                "Notez-le et gardez-le en lieu sûr. Il ne sera plus jamais affiché.",
                "J'ai noté mon code");

            await _nav.GoToAsync(Routes.Racine(Routes.Connexion));
        }
        catch (AuthentificationException ex)
        {
            MessageErreur = ex.Message; // les champs saisis sont conservés
        }
        catch (Exception ex)
        {
            MessageErreur = "La réinitialisation a échoué : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
