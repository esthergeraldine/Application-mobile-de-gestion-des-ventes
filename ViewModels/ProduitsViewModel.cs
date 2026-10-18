using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class ProduitsViewModel : BaseViewModel
{
    private readonly IProduitService _produits;
    private readonly INavigationService _nav;
    private List<Produit> _tous = new();
    private string _categorieActive;

    public ProduitsViewModel(IProduitService produits, INavigationService nav)
    {
        _produits = produits;
        _nav = nav;
    }

    public ObservableCollection<Produit> Produits { get; } = new();
    public ObservableCollection<ChipItem> Chips { get; } = new();

    [ObservableProperty] private string recherche;

    public bool EstVide => _tous.Count == 0;
    public bool PasVide => _tous.Count > 0;

    partial void OnRechercheChanged(string value) => Filtrer();

    [RelayCommand]
    private async Task ChargerAsync()
    {
        _tous = (await _produits.GetProduitsAsync()).ToList();

        Chips.Clear();
        Chips.Add(new ChipItem { Cle = null, Libelle = $"Tous({_tous.Count})", EstActive = _categorieActive == null });
        foreach (var cat in _tous.Select(p => p.Categorie).Distinct())
            Chips.Add(new ChipItem { Cle = cat, Libelle = cat, EstActive = cat == _categorieActive });

        OnPropertyChanged(nameof(EstVide));
        OnPropertyChanged(nameof(PasVide));
        Filtrer();
    }

    [RelayCommand]
    private void Selectionner(ChipItem chip)
    {
        if (chip == null) return;
        _categorieActive = chip.Cle;
        foreach (var c in Chips)
            c.EstActive = c == chip;
        Filtrer();
    }

    private void Filtrer()
    {
        IEnumerable<Produit> resultat = _tous;
        if (_categorieActive != null)
            resultat = resultat.Where(p => p.Categorie == _categorieActive);
        if (!string.IsNullOrWhiteSpace(Recherche))
            resultat = resultat.Where(p => p.Nom.Contains(Recherche.Trim(), StringComparison.OrdinalIgnoreCase));

        Produits.Clear();
        foreach (var p in resultat)
            Produits.Add(p);
    }

    [RelayCommand]
    private Task AjouterAsync() => _nav.GoToAsync(Routes.AjouterProduit);

    [RelayCommand]
    private Task ModifierAsync(Produit produit)
        => produit == null ? Task.CompletedTask : _nav.GoToAsync($"{Routes.AjouterProduit}?id={produit.Id}");

    [RelayCommand]
    private Task RetourAsync() => _nav.GoToAsync(Routes.Racine(Routes.Accueil));
}
