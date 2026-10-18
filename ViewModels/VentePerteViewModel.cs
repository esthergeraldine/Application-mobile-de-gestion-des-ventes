using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Helpers;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class VentePerteViewModel : VenteFormViewModelBase, IQueryAttributable
{
    private static readonly Color RougeFond = Color.FromArgb("#FFEFEF");
    private static readonly Color MentheFond = Color.FromArgb("#E3F4EE");
    private static readonly Color Rouge = Color.FromArgb("#D32F2F");
    private static readonly Color Vert = Color.FromArgb("#006652");

    public VentePerteViewModel(IProduitService produitService, IVenteService venteService,
                               INavigationService nav, IDialogService dialog)
        : base(produitService, venteService, nav, dialog)
    {
    }

    [ObservableProperty] private bool confirme;

    partial void OnConfirmeChanged(bool value) => OnPropertyChanged(nameof(BoutonTexte));

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("produitId", out var id) && id != null)
            ProduitIdInitial = Convert.ToInt32(id, CultureInfo.InvariantCulture);
        if (query.TryGetValue("quantite", out var q) && q != null)
            Quantite = Math.Max(1, Convert.ToInt32(q, CultureInfo.InvariantCulture));
    }

    public bool AfficherConfirmation => EstVenteAPerte;
    public string LibelleResultat => EstVenteAPerte ? "Perte" : "Bénéfice";
    public Color FondResultat => EstVenteAPerte ? RougeFond : MentheFond;
    public Color CouleurResultat => EstVenteAPerte ? Rouge : Vert;

    public string ResultatTexte =>
        $"{(Benefice > 0 ? "+" : string.Empty)}{Format.Fcfa(Benefice)} ({Format.Pourcentage(Math.Abs(MargePourcentage))} %)";

    public string BoutonTexte => !EstVenteAPerte ? "Valider la vente"
                               : Confirme ? "Valider la vente à perte"
                               : "Ajuster le prix";

    protected override void Rafraichir()
    {
        base.Rafraichir();
        OnPropertyChanged(nameof(AfficherConfirmation));
        OnPropertyChanged(nameof(LibelleResultat));
        OnPropertyChanged(nameof(FondResultat));
        OnPropertyChanged(nameof(CouleurResultat));
        OnPropertyChanged(nameof(ResultatTexte));
        OnPropertyChanged(nameof(BoutonTexte));
    }

    [RelayCommand]
    private async Task ActionPrincipaleAsync()
    {
        if (ProduitSelectionne == null)
            return;

        // Non confirmé : on propose d'ajuster le prix (ouvre la fiche produit)
        if (EstVenteAPerte && !Confirme)
        {
            await Nav.GoToAsync($"{Routes.AjouterProduit}?id={ProduitSelectionne.Id}");
            return;
        }

        if (!await VerifierAsync())
            return;

        await EnregistrerEtAfficherSuccesAsync();
    }
}
