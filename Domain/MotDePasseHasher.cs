using System.Security.Cryptography;

namespace VenteJour.Domain;

/// <summary>Empreinte de mot de passe : PBKDF2-SHA256, sel aléatoire de 16 octets, 100 000 itérations.</summary>
public static class MotDePasseHasher
{
    public const int IterationsParDefaut = 100_000;
    private const int TailleSel = 16;
    private const int TailleHash = 32;

    public static (string Sel, string Hash, int Iterations) Hasher(string motDePasse)
    {
        var sel = RandomNumberGenerator.GetBytes(TailleSel);
        var hash = Rfc2898DeriveBytes.Pbkdf2(motDePasse, sel, IterationsParDefaut, HashAlgorithmName.SHA256, TailleHash);
        return (Convert.ToBase64String(sel), Convert.ToBase64String(hash), IterationsParDefaut);
    }

    public static bool Verifier(string motDePasse, string sel, string hash, int iterations)
    {
        if (string.IsNullOrEmpty(motDePasse) || string.IsNullOrEmpty(sel) || string.IsNullOrEmpty(hash) || iterations <= 0)
            return false;
        try
        {
            var attendu = Convert.FromBase64String(hash);
            var calcule = Rfc2898DeriveBytes.Pbkdf2(motDePasse, Convert.FromBase64String(sel), iterations, HashAlgorithmName.SHA256, attendu.Length);
            return CryptographicOperations.FixedTimeEquals(calcule, attendu); // comparaison à temps constant
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
