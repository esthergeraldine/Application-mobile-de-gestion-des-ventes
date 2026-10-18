using System.Globalization;

namespace VenteJour.Helpers;

public class FcfaConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int i ? Format.Fcfa(i) : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class NombreConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int i ? Format.Nombre(i) : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>"drop" -> "ico_drop_g" (icône verte). Icône par défaut si null.</summary>
public class IconKeyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var cle = value as string;
        return string.IsNullOrEmpty(cle) ? "ico_drop_g" : $"ico_{cle}_g";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Rang 1 = orange, 2 = vert menthe, 3 = bleu (comme la maquette).</summary>
public class RangCouleurConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is int rang ? rang : 0) switch
        {
            1 => Color.FromArgb("#F28C28"),
            2 => Color.FromArgb("#2FCB9C"),
            _ => Color.FromArgb("#1E88C8")
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>"box" -> "ico_box_w" (icône blanche, pour les pastilles vertes).</summary>
public class IconKeyBlancConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var cle = value as string;
        return string.IsNullOrEmpty(cle) ? "ico_box_w" : $"ico_{cle}_w";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>true si la chaîne n'est ni nulle ni vide (affichage conditionnel des messages d'erreur).</summary>
public class NotEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is string s && !string.IsNullOrEmpty(s);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : true;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : false;
}
