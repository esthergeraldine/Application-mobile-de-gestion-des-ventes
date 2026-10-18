using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Helpers;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public class VenteListItem
{
    public string Id { get; set; }
    public string Libelle { get; set; }
    public string Icone { get; set; }
    public int Total { get; set; }
    public string Heure { get; set; }
    public bool EstAnnulee { get; set; }
}

public partial class HistoriqueViewModel : BaseViewModel
{
    private readonly IVenteService _ventes;
    private readonly INavigationService _nav;
    private PeriodeVente _periode = PeriodeVente.Aujourdhui;

    public HistoriqueViewModel(IVenteService ventes, INavigationService nav)
    {
        _ventes = ventes;
        _nav = nav;
        Chips.Add(new ChipItem { Cle = nameof(PeriodeVente.Aujourdhui), Libelle = "Aujourd'hui", EstActive = true });
        Chips.Add(new ChipItem { Cle = nameof(PeriodeVente.Hier), Libelle = "Hier" });
        Chips.Add(new ChipItem { Cle = nameof(PeriodeVente.Semaine), Libelle = "Semaine" });
    }

    public ObservableCollection<ChipItem> Chips { get; } = new();
    public ObservableCollection<VenteListItem> Ventes { get; } = new();

    [ObservableProperty] private string totalTexte = "0 FCFA";
    [ObservableProperty] private string variationTexte;
    [ObservableProperty] private bool aVariation;
    [ObservableProperty] private Color variationCouleur = Color.FromArgb("#2FCB9C");
    [ObservableProperty] private bool estVide;

    [RelayCommand]
    private async Task ChargerAsync()
    {
        var historique = await _ventes.GetHistoriqueAsync(_periode);

        TotalTexte = Format.Fcfa(historique.Total);
        AVariation = historique.VariationPourcentage.HasValue;
        if (historique.VariationPourcentage.HasValue)
        {
            var v = historique.VariationPourcentage.Value;
            VariationTexte = Format.PourcentageSigne(v);
            VariationCouleur = v >= 0 ? Color.FromArgb("#2FCB9C") : Color.FromArgb("#D32F2F");
        }

        var format = _periode == PeriodeVente.Semaine ? "dd/MM HH:mm" : "HH:mm";
        Ventes.Clear();
        foreach (var v in historique.Ventes)
        {
            Ventes.Add(new VenteListItem
            {
                Id = v.Id, Libelle = v.Libelle, Icone = v.Icone, Total = v.Total,
                Heure = v.Date.ToString(format, CultureInfo.InvariantCulture) + (v.EstValide ? string.Empty : " · Annulée"),
                EstAnnulee = !v.EstValide
            });
        }
        EstVide = Ventes.Count == 0;
    }

    [RelayCommand]
    private async Task SelectionnerAsync(ChipItem chip)
    {
        if (chip == null) return;
        _periode = Enum.Parse<PeriodeVente>(chip.Cle);
        foreach (var c in Chips)
            c.EstActive = c == chip;
        await ChargerAsync();
    }

    [RelayCommand]
    private Task OuvrirAsync(VenteListItem vente)
        => vente == null ? Task.CompletedTask : _nav.GoToAsync($"{Routes.Detail}?id={vente.Id}");
}
