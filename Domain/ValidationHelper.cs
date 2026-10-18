using System.Text.RegularExpressions;

namespace VenteJour.Domain;

public static class ValidationHelper
{
    private static readonly Regex Email = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    /// <summary>Retire tout sauf les chiffres, et l'indicatif 237 s'il est présent.</summary>
    public static string ChiffresTelephone(string telephone)
    {
        var chiffres = new string((telephone ?? string.Empty).Where(char.IsDigit).ToArray());
        if (chiffres.Length == 12 && chiffres.StartsWith("237"))
            chiffres = chiffres.Substring(3);
        return chiffres;
    }

    /// <summary>Mobile camerounais : 9 chiffres commençant par 6 (avec ou sans +237).</summary>
    public static bool TelephoneValide(string telephone)
    {
        var c = ChiffresTelephone(telephone);
        return c.Length == 9 && c[0] == '6';
    }

    /// <summary>699123456 → "6 99 12 34 56".</summary>
    public static string FormaterTelephone(string telephone)
    {
        var c = ChiffresTelephone(telephone);
        if (c.Length != 9) return telephone ?? string.Empty;
        return $"{c.Substring(0, 1)} {c.Substring(1, 2)} {c.Substring(3, 2)} {c.Substring(5, 2)} {c.Substring(7, 2)}";
    }

    public static bool EmailValide(string email) => !string.IsNullOrWhiteSpace(email) && Email.IsMatch(email.Trim());

    public static bool ContactValide(string contact) => EmailValide(contact) || TelephoneValide(contact);
}
