using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Helpers;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class SuccesViewModel : BaseViewModel, IQueryAttributable
{
    private readonly INavigationService _nav;

    public SuccesViewModel(INavigationService nav) => _nav = nav;

    [ObservableProperty] private string montantTexte = "0 FCFA";
    [ObservableProperty] private string beneficeTexte = "0 FCFA";
    [ObservableProperty] private string variationTexte = "+0%";
    [ObservableProperty] private Color variationCouleur = Color.FromArgb("#2FCB9C");

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("montant", out var m) && m != null)
            MontantTexte = Format.Fcfa(Convert.ToInt32(m, CultureInfo.InvariantCulture));

        if (query.TryGetValue("benefice", out var b) && b != null)
            BeneficeTexte = Format.Fcfa(Convert.ToInt32(b, CultureInfo.InvariantCulture));

        if (query.TryGetValue("marge", out var p) && p != null &&
            double.TryParse(Convert.ToString(p, CultureInfo.InvariantCulture), NumberStyles.Float,
                            CultureInfo.InvariantCulture, out var marge))
        {
            VariationTexte = Format.PourcentageSigne(marge);
            VariationCouleur = marge >= 0 ? Color.FromArgb("#2FCB9C") : Color.FromArgb("#D32F2F");
        }
    }

    [RelayCommand]
    private async Task AutreVenteAsync()
    {
        await _nav.GoToAsync(Routes.Racine(Routes.Accueil));
        await _nav.GoToAsync(Routes.NouvelleVente);
    }

    [RelayCommand]
    private Task AccueilAsync() => _nav.GoToAsync(Routes.Racine(Routes.Accueil));
}
