namespace VenteJour.Services;

/// <summary>Mémorise le compte à rouvrir (« Se souvenir de moi ») dans les Preferences du téléphone.</summary>
public class PreferencesStockageSession : IStockageSession
{
    private const string Cle = "compte_memorise";

    public int? LireCompteMemorise()
    {
        var id = Preferences.Default.Get(Cle, 0);
        return id > 0 ? id : null;
    }

    public void MemoriserCompte(int compteId) => Preferences.Default.Set(Cle, compteId);

    public void Oublier() => Preferences.Default.Remove(Cle);
}
