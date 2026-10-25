using System.Security.Cryptography;

namespace VenteJour.Domain;

/// <summary>
/// Code de récupération de compte (comme les codes de secours d'un service en ligne) : 12 caractères tirés au hasard,
/// sans caractères ambigus (pas de 0/O ni de 1/I/L), affichés par groupes de 4 : « K7M2-9QXA-R4TD ».
/// Il est montré UNE fois à l'utilisateur ; seule son empreinte est conservée.
/// </summary>
public static class CodeRecuperation
{
    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    public static string Generer()
    {
        var caracteres = new char[12];
        for (int i = 0; i < caracteres.Length; i++)
            caracteres[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        var brut = new string(caracteres);
        return $"{brut.Substring(0, 4)}-{brut.Substring(4, 4)}-{brut.Substring(8, 4)}";
    }

    /// <summary>« k7m2 9qxa-r4td » → « K7M29QXAR4TD » (insensible aux espaces, tirets et majuscules).</summary>
    public static string Normaliser(string code)
        => new string((code ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
}
