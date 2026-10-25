namespace VenteJour.Services;

/// <summary>Cache local sur le téléphone (Preferences MAUI), comme Preferences.Set("taux_eur", ...) dans le TP.</summary>
public class PreferencesCache : ICacheLocal
{
    public bool TryLire(string cle, out double valeur)
    {
        valeur = Preferences.Default.Get(cle, 0.0);
        return Preferences.Default.ContainsKey(cle);
    }

    public void Ecrire(string cle, double valeur) => Preferences.Default.Set(cle, valeur);
}
