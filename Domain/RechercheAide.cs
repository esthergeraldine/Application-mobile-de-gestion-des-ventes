using System.Globalization;
using System.Text;
using VenteJour.Models;

namespace VenteJour.Domain;

public static class RechercheAide
{
    /// <summary>Minuscules + suppression des accents : "Bénéfices" → "benefices".</summary>
    public static string Normaliser(string texte)
    {
        if (string.IsNullOrWhiteSpace(texte)) return string.Empty;
        var decompose = texte.Trim().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decompose.Length);
        foreach (var c in decompose)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().ToLowerInvariant().Replace("œ", "oe").Replace("æ", "ae");
    }

    /// <summary>Un article correspond si chaque mot saisi figure dans son titre, sa catégorie, son résumé, son contenu ou ses mots-clés.</summary>
    public static IReadOnlyList<ArticleAide> Filtrer(IEnumerable<ArticleAide> articles, string texte)
    {
        var mots = Normaliser(texte).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (mots.Length == 0) return new List<ArticleAide>();

        return articles.Where(a =>
        {
            var cible = Normaliser($"{a.Titre} {a.CategorieTitre} {a.Resume} {string.Join(' ', a.Paragraphes)} {string.Join(' ', a.MotsCles)}");
            return mots.All(m => cible.Contains(m));
        }).ToList();
    }
}
