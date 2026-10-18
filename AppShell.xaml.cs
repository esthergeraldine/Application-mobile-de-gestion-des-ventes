using VenteJour.Services;
using VenteJour.Views;

namespace VenteJour;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Pages empilées (push) au-dessus des pages racines
        Routing.RegisterRoute(Routes.NouvelleVente, typeof(NouvelleVentePage));
        Routing.RegisterRoute(Routes.VentePerte, typeof(VentePertePage));
        Routing.RegisterRoute(Routes.Succes, typeof(SuccesPage));
        Routing.RegisterRoute(Routes.AjouterProduit, typeof(AjouterProduitPage));
        Routing.RegisterRoute(Routes.Detail, typeof(DetailTransactionPage));
        Routing.RegisterRoute(Routes.Parametres, typeof(ParametresPage));
        Routing.RegisterRoute(Routes.Kiosque, typeof(KiosquePage));
        Routing.RegisterRoute(Routes.Classement, typeof(ClassementPage));
        Routing.RegisterRoute(Routes.Aide, typeof(AidePage));
        Routing.RegisterRoute(Routes.AideCategorie, typeof(AideCategoriePage));
        Routing.RegisterRoute(Routes.AideArticle, typeof(AideArticlePage));
        Routing.RegisterRoute(Routes.ContactSupport, typeof(ContactSupportPage));
        Routing.RegisterRoute(Routes.SignalerProbleme, typeof(SignalerProblemePage));
    }
}
