using VenteJour.Helpers;

namespace VenteJour.Services;

public class ParametresService : IParametresService
{
    public bool AlertesVenteAPerte
    {
        get => Preferences.Default.Get(nameof(AlertesVenteAPerte), true);
        set => Preferences.Default.Set(nameof(AlertesVenteAPerte), value);
    }

    public bool RappelsCloture
    {
        get => Preferences.Default.Get(nameof(RappelsCloture), true);
        set => Preferences.Default.Set(nameof(RappelsCloture), value);
    }

    public bool ThemeSombre
    {
        get => Preferences.Default.Get(nameof(ThemeSombre), false);
        set
        {
            Preferences.Default.Set(nameof(ThemeSombre), value);
            AppliquerTheme();
        }
    }

    public string Langue
    {
        get => Preferences.Default.Get(nameof(Langue), "Français");
        set => Preferences.Default.Set(nameof(Langue), value);
    }

    public void AppliquerTheme() => ThemeHelper.Appliquer(ThemeSombre);
}
