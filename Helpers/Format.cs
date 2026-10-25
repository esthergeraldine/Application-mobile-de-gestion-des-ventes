using System.Globalization;

namespace VenteJour.Helpers;

public static class Format
{
    private static readonly CultureInfo Fr = new("fr-FR");

    public static string Nombre(int valeur)
        => valeur.ToString("N0", Fr).Replace('\u202F', '\u00A0');

    public static string Fcfa(int valeur) => $"{Nombre(valeur)} FCFA";

    /// <summary>1 234,5 + "EUR" → "1 234,50 EUR".</summary>
    public static string Devise(decimal montant, string code)
        => $"{montant.ToString("N2", Fr).Replace('\u202F', '\u00A0')} {code}";

    public static string Pourcentage(double valeur)
        => valeur.ToString("0.0", CultureInfo.InvariantCulture);

    public static string PourcentageSigne(double valeur)
        => (valeur >= 0 ? "+" : "-") + Math.Abs(valeur).ToString("0.#", CultureInfo.InvariantCulture) + "%";
}
