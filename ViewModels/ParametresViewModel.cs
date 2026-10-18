using CommunityToolkit.Mvvm.ComponentModel;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class ParametresViewModel : BaseViewModel
{
    private readonly IParametresService _parametres;

    public ParametresViewModel(IParametresService parametres)
    {
        _parametres = parametres;
        alertesVenteAPerte = parametres.AlertesVenteAPerte;
        rappelsCloture = parametres.RappelsCloture;
        themeSombre = parametres.ThemeSombre;
        langue = parametres.Langue;
    }

    public string[] Langues { get; } = { "Français", "English" };

    [ObservableProperty] private bool alertesVenteAPerte;
    [ObservableProperty] private bool rappelsCloture;
    [ObservableProperty] private bool themeSombre;
    [ObservableProperty] private string langue;

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
}
