namespace VenteJour.Domain;

/// <summary>
/// Freine les essais répétés : après 5 échecs consécutifs sur une même clé (un compte), blocage d'une minute.
/// Protection en mémoire : elle ralentit les essais au hasard mais se remet à zéro si l'application est relancée.
/// </summary>
public class LimiteurTentatives
{
    private readonly int _maximum;
    private readonly TimeSpan _duree;
    private readonly Func<DateTime> _maintenant;
    private readonly Dictionary<string, (int Echecs, DateTime? BloqueJusqua)> _etat = new();
    private readonly object _verrou = new();

    public LimiteurTentatives(int maximum = 5, TimeSpan? duree = null, Func<DateTime> maintenant = null)
    {
        _maximum = maximum;
        _duree = duree ?? TimeSpan.FromMinutes(1);
        _maintenant = maintenant ?? (() => DateTime.UtcNow);
    }

    /// <summary>Temps restant avant de pouvoir réessayer, ou null si ce n'est pas bloqué.</summary>
    public TimeSpan? TempsRestant(string cle)
    {
        lock (_verrou)
        {
            if (_etat.TryGetValue(cle, out var e) && e.BloqueJusqua.HasValue)
            {
                var reste = e.BloqueJusqua.Value - _maintenant();
                if (reste > TimeSpan.Zero) return reste;
                _etat.Remove(cle); // blocage terminé : on repart de zéro
            }
            return null;
        }
    }

    public void EnregistrerEchec(string cle)
    {
        lock (_verrou)
        {
            _etat.TryGetValue(cle, out var e);
            int echecs = e.Echecs + 1;
            _etat[cle] = echecs >= _maximum ? (0, _maintenant() + _duree) : (echecs, null);
        }
    }

    public void Reinitialiser(string cle)
    {
        lock (_verrou) { _etat.Remove(cle); }
    }
}
