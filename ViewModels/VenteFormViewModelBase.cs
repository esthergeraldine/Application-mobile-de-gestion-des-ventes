using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Helpers;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

/// <summary>Logique commune aux écrans "Nouvelle vente" et "Vente à perte".</summary>
public abstract partial class VenteFormViewModelBase : BaseViewModel
{
    protected readonly IProduitService ProduitService;
    protected readonly IVenteService VenteService;
    protected readonly INavigationService Nav;
    protected readonly IDialogService Dialog;

    protected VenteFormViewModelBase(IProduitService produitService, IVenteService venteService,
                                     INavigationService nav, IDialogService dialog)
    {
        ProduitService = produitService;
        VenteService = venteService;
        Nav = nav;
        Dialog = dialog;
    }

    public ObservableCollection<Produit> Produits { get; } = new();

    [ObservableProperty] private Produit produitSelectionne;
    [ObservableProperty] private int quantite = 1;

    protected int? ProduitIdInitial { get; set; }

    partial void OnProduitSelectionneChanged(Produit value) => Rafraichir();
    partial void OnQuantiteChanged(int value) => Rafraichir();

    public string PrixAchatTexte => Format.Fcfa(ProduitSelectionne?.PrixAchat ?? 0);
    public string PrixVenteTexte => Format.Fcfa(ProduitSelectionne?.PrixVente ?? 0);
    public int Total => (ProduitSelectionne?.PrixVente ?? 0) * Quantite;
    public string TotalTexte => Format.Fcfa(Total);
    public int Benefice => Total - (ProduitSelectionne?.PrixAchat ?? 0) * Quantite;
    public double MargePourcentage => Total == 0 ? 0 : Benefice * 100.0 / Total;
    public bool EstVenteAPerte => ProduitSelectionne != null && ProduitSelectionne.EstVenteAPerte;
    public string StockTexte => $"Stock disponible : {ProduitSelectionne?.Stock ?? 0}";

    protected virtual void Rafraichir()
    {
        OnPropertyChanged(nameof(PrixAchatTexte));
        OnPropertyChanged(nameof(PrixVenteTexte));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalTexte));
        OnPropertyChanged(nameof(Benefice));
        OnPropertyChanged(nameof(MargePourcentage));
        OnPropertyChanged(nameof(EstVenteAPerte));
        OnPropertyChanged(nameof(StockTexte));
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        // On mémorise la sélection avant de vider la liste (le Picker remet la sélection à null)
        var idCourant = ProduitSelectionne?.Id ?? ProduitIdInitial;

        var liste = await ProduitService.GetProduitsAsync();
        Produits.Clear();
        foreach (var p in liste)
            Produits.Add(p);

        ProduitSelectionne = Produits.FirstOrDefault(p => p.Id == idCourant) ?? Produits.FirstOrDefault();

        if (ProduitSelectionne != null && Quantite > ProduitSelectionne.Stock)
            Quantite = Math.Max(1, ProduitSelectionne.Stock);
        else
            Rafraichir();
    }

    [RelayCommand]
    private void Incrementer()
    {
        if (ProduitSelectionne == null || Quantite < ProduitSelectionne.Stock)
            Quantite++;
    }

    [RelayCommand]
    private void Decrementer()
    {
        if (Quantite > 1)
            Quantite--;
    }

    /// <summary>Contrôles communs avant d'enregistrer. Retourne false si la vente est impossible.</summary>
    protected async Task<bool> VerifierAsync()
    {
        if (ProduitSelectionne == null)
        {
            await Dialog.AlertAsync("Article manquant", "Veuillez choisir un article.");
            return false;
        }
        if (Quantite > ProduitSelectionne.Stock)
        {
            await Dialog.AlertAsync("Stock insuffisant", $"Il ne reste que {ProduitSelectionne.Stock} en stock.");
            return false;
        }
        return true;
    }

    protected async Task EnregistrerEtAfficherSuccesAsync()
    {
        var p = ProduitSelectionne;
        var ligne = new LigneVente
        {
            ProduitId = p.Id, NomProduit = p.Nom, Icone = p.Icone, Quantite = Quantite,
            PrixAchatUnitaire = p.PrixAchat, PrixVenteUnitaire = p.PrixVente
        };

        IsBusy = true;
        try
        {
            var vente = await VenteService.EnregistrerVenteAsync(new[] { ligne }, "Espèces");
            var marge = vente.MargePourcentage.ToString("0.0", CultureInfo.InvariantCulture);
            await Nav.GoToAsync($"{Routes.Succes}?montant={vente.Total}&benefice={vente.Benefice}&marge={marge}");
        }
        catch (InvalidOperationException ex)
        {
            await Dialog.AlertAsync("Vente impossible", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
