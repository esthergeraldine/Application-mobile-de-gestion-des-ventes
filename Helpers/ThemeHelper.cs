using VenteJour.Resources.Styles;

namespace VenteJour.Helpers;

public static class ThemeHelper
{
    public static void Appliquer(bool sombre)
    {
        var dictionnaires = Application.Current?.Resources?.MergedDictionaries;
        if (dictionnaires == null)
            return;

        var existant = dictionnaires.FirstOrDefault(d => d is ColorsLight || d is ColorsDark);
        if (existant != null)
            dictionnaires.Remove(existant);

        dictionnaires.Add(sombre ? new ColorsDark() : new ColorsLight());
    }
}
