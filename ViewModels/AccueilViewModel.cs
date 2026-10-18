using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Helpers;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class AccueilViewModel : BaseViewModel
{
    private readonly IBilanService _bilan;
    private readonly IClassementService _classement;
    private readonly INavigationService _nav;

    public AccueilViewModel(IBilanService bilan, IClassementService classement, INavigationService nav)
    {
        _bilan = bilan;
        _classement = classement;
        _nav = nav;
    }

    [ObservableProperty] private string recetteTexte = "0";
    [ObservableProperty] private string beneficeTexte = "0 FCFA";
    [ObservableProperty] private string margeTexte = "Marge 0 %";
    [ObservableProperty] private int transactions;
    [ObservableProperty] private int articlesSortis;

    public ObservableCollection<ClassementProduit> Top { get; } = new();

    [RelayCommand]
    private async Task ChargerAsync()
    {
        var bilan = await _bilan.GetBilanAsync(DateTime.Today);
        RecetteTexte = Format.Nombre(bilan.Recette);
        BeneficeTexte = (bilan.Benefice >= 0 ? "+" : "") + Format.Fcfa(bilan.Benefice);
        MargeTexte = $"Marge {Format.Pourcentage(bilan.MargePourcentage)} %";
        Transactions = bilan.NombreTransactions;
        ArticlesSortis = bilan.ArticlesSortis;

        var top = await _classement.GetClassementAsync(CritereClassement.Recettes,
            PlageDates.Pour(PeriodeClassement.Aujourdhui, DateTime.Now), 3);
        Top.Clear();
        foreach (var c in top)
            Top.Add(c);
    }

    [RelayCommand]
    private Task NouvelleVenteAsync() => _nav.GoToAsync(Routes.NouvelleVente);

    [RelayCommand]
    private Task VoirToutAsync() => _nav.GoToAsync(Routes.Classement);
}
