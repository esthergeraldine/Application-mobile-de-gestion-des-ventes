using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Services;
using VenteJour.Services.Api;

namespace VenteJour.ViewModels;

public partial class ParametresViewModel : BaseViewModel
{
    private readonly IParametresService _parametres;
    private readonly IDonneesDemoService _demo;
    private readonly IDialogService _dialog;

    public ParametresViewModel(IParametresService parametres, IDonneesDemoService demo, IDialogService dialog)
    {
        _parametres = parametres;
        _demo = demo;
        _dialog = dialog;
        alertesVenteAPerte = parametres.AlertesVenteAPerte;
        rappelsCloture = parametres.RappelsCloture;
        themeSombre = parametres.ThemeSombre;
        langue = parametres.Langue;
        simulerPanneReseau = FakeApiHandler.SimulerPanne;
    }

    public string[] Langues { get; } = { "Français", "English" };

    [ObservableProperty] private bool alertesVenteAPerte;
    [ObservableProperty] private bool rappelsCloture;
    [ObservableProperty] private bool themeSombre;
    [ObservableProperty] private string langue;

    // Démonstration phase B : coupe le « réseau » du faux serveur pour montrer le mode hors ligne
    [ObservableProperty] private bool simulerPanneReseau;

    partial void OnSimulerPanneReseauChanged(bool value) => FakeApiHandler.SimulerPanne = value;

    public bool ThemeClair
    {
        get => !ThemeSombre;
        set { if (value) ThemeSombre = false; }
    }

    partial void OnAlertesVenteAPerteChanged(bool value) => _parametres.AlertesVenteAPerte = value;
    partial void OnRappelsClotureChanged(bool value) => _parametres.RappelsCloture = value;

    partial void OnThemeSombreChanged(bool value)
    {
        _parametres.ThemeSombre = value;
        OnPropertyChanged(nameof(ThemeClair));
    }

    partial void OnLangueChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _parametres.Langue = value;
    }

    [RelayCommand]
    private async Task ChargerDemoAsync()
    {
        if (IsBusy) return;
        var confirme = await _dialog.ConfirmAsync("Données de démonstration",
            "Les produits et les ventes de CE compte seront remplacés par des données d'exemple (pour les captures d'écran et la démonstration). Continuer ?",
            "Remplacer", "Annuler");
        if (!confirme) return;

        IsBusy = true;
        try
        {
            await _demo.ChargerAsync();
            await _dialog.AlertAsync("Données chargées", "Des produits et des ventes d'exemple ont été ajoutés à ce compte (enregistrés sur cet appareil uniquement).");
        }
        catch (Exception ex)
        {
            await _dialog.AlertAsync("Échec", "Le chargement a échoué : " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
