using CommunityToolkit.Mvvm.Input;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class NouvelleVenteViewModel : VenteFormViewModelBase
{
    public NouvelleVenteViewModel(IProduitService produitService, IVenteService venteService,
                                  INavigationService nav, IDialogService dialog)
        : base(produitService, venteService, nav, dialog)
    {
    }

    [RelayCommand]
    private async Task ValiderAsync()
    {
        if (!await VerifierAsync())
            return;

        // Prix de vente < prix d'achat : passage par l'écran de confirmation "Vente à perte"
        if (ProduitSelectionne.EstVenteAPerte)
        {
            await Nav.GoToAsync($"{Routes.VentePerte}?produitId={ProduitSelectionne.Id}&quantite={Quantite}");
            return;
        }

        await EnregistrerEtAfficherSuccesAsync();
    }
}
