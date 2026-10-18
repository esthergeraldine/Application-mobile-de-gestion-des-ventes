using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class AjouterProduitViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IProduitService _produits;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;
    private int? _produitId;
    private bool _charge;

    public AjouterProduitViewModel(IProduitService produits, INavigationService nav, IDialogService dialog)
    {
        _produits = produits;
        _nav = nav;
        _dialog = dialog;
    }

    public ObservableCollection<string> Categories { get; } = new();

    [ObservableProperty] private string titre = "Ajouter un produit";
    [ObservableProperty] private string nom;
    [ObservableProperty] private string categorie;
    [ObservableProperty] private string prixAchatTexte;
    [ObservableProperty] private string prixVenteTexte;
    [ObservableProperty] private string stockTexte;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var valeur) && valeur != null)
            _produitId = Convert.ToInt32(valeur, CultureInfo.InvariantCulture);
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        if (_charge) return;
        _charge = true;

        foreach (var c in await _produits.GetCategoriesAsync())
            Categories.Add(c);

        if (_produitId.HasValue)
        {
            var p = await _produits.GetProduitAsync(_produitId.Value);
            if (p != null)
            {
                Titre = "Modifier le produit";
                Nom = p.Nom;
                Categorie = p.Categorie;
                PrixAchatTexte = p.PrixAchat.ToString(CultureInfo.InvariantCulture);
                PrixVenteTexte = p.PrixVente.ToString(CultureInfo.InvariantCulture);
                StockTexte = p.Stock.ToString(CultureInfo.InvariantCulture);
            }
        }
        else
        {
            Categorie = Categories.FirstOrDefault();
        }
    }

    [RelayCommand]
    private async Task EnregistrerAsync()
    {
        if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Categorie))
        {
            await _dialog.AlertAsync("Champs manquants", "Le nom et la catégorie sont obligatoires.");
            return;
        }
        if (!int.TryParse(PrixAchatTexte, out var achat) || !int.TryParse(PrixVenteTexte, out var vente) ||
            !int.TryParse(StockTexte, out var stock) || achat < 0 || vente < 0 || stock < 0)
        {
            await _dialog.AlertAsync("Valeurs invalides", "Saisissez des nombres entiers positifs pour les prix et le stock.");
            return;
        }

        var produit = new Produit
        {
            Id = _produitId ?? 0,
            Nom = Nom.Trim(),
            Categorie = Categorie,
            PrixAchat = achat,
            PrixVente = vente,
            Stock = stock,
            Icone = ChoisirIcone(Nom, Categorie)
        };

        if (_produitId.HasValue)
            await _produits.MettreAJourAsync(produit);
        else
            await _produits.AjouterAsync(produit);

        await _nav.GoBackAsync();
    }

    [RelayCommand]
    private Task ChoisirPhotoAsync()
        => _dialog.AlertAsync("Photo", "L'ajout de photo sera disponible avec la future API.");

    private static string ChoisirIcone(string nom, string categorie)
    {
        var n = nom.ToLowerInvariant();
        if (n.Contains("eau")) return "drop";
        if (n.Contains("jus") || n.Contains("soda")) return "cup";
        if (n.Contains("biscuit")) return "cookie";
        if (n.Contains("pain")) return "bread";
        return categorie == "Boissons" ? "cup" : "box";
    }
}
