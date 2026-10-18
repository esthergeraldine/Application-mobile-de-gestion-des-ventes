using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using VenteJour.Services;
using VenteJour.Services.Fake;
using VenteJour.ViewModels;
using VenteJour.Views;

namespace VenteJour;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

#if ANDROID
        // Supprime le soulignement Material des Entry et Picker (champs encadrés comme dans la maquette)
        EntryHandler.Mapper.AppendToMapping("SansSoulignement", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });
        DatePickerHandler.Mapper.AppendToMapping("SansSoulignement", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });
        PickerHandler.Mapper.AppendToMapping("SansSoulignement", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });
#endif

        // ------------------------------------------------------------------
        // Services de données
        // Pour brancher la future API ASP.NET Core, remplacez uniquement
        // ces enregistrements par des implémentations HTTP (voir Services/Api).
        // Aucun ViewModel ni aucune page ne change.
        // ------------------------------------------------------------------
        builder.Services.AddSingleton<FakeDataStore>();
        builder.Services.AddSingleton<IProduitService, FakeProduitService>();
        builder.Services.AddSingleton<IVenteService, FakeVenteService>();
        builder.Services.AddSingleton<IBilanService, FakeBilanService>();
        builder.Services.AddSingleton<IAuthService, FakeAuthService>();
        builder.Services.AddSingleton<IClassementService, FakeClassementService>();
        builder.Services.AddSingleton<IKiosqueService, FakeKiosqueService>();
        builder.Services.AddSingleton<ILocalisationService, FakeLocalisationService>();
        builder.Services.AddSingleton<IAideService, FakeAideService>();
        builder.Services.AddSingleton<ISupportService, FakeSupportService>();
        // Exemple futur :
        // builder.Services.AddHttpClient<IProduitService, Services.Api.HttpProduitService>(
        //     c => c.BaseAddress = new Uri("https://votre-api.exemple.com/"));

        // Services applicatifs
        builder.Services.AddSingleton<IParametresService, ParametresService>();
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<ISelecteurFichierService, SelecteurFichierService>();

        // ViewModels
        builder.Services.AddTransient<ConnexionViewModel>();
        builder.Services.AddTransient<InscriptionViewModel>();
        builder.Services.AddTransient<AccueilViewModel>();
        builder.Services.AddTransient<ProduitsViewModel>();
        builder.Services.AddTransient<AjouterProduitViewModel>();
        builder.Services.AddTransient<NouvelleVenteViewModel>();
        builder.Services.AddTransient<VentePerteViewModel>();
        builder.Services.AddTransient<SuccesViewModel>();
        builder.Services.AddTransient<HistoriqueViewModel>();
        builder.Services.AddTransient<DetailTransactionViewModel>();
        builder.Services.AddTransient<ProfilViewModel>();
        builder.Services.AddTransient<ParametresViewModel>();
        builder.Services.AddTransient<KiosqueViewModel>();
        builder.Services.AddTransient<ClassementViewModel>();
        builder.Services.AddTransient<AideViewModel>();
        builder.Services.AddTransient<AideCategorieViewModel>();
        builder.Services.AddTransient<AideArticleViewModel>();
        builder.Services.AddTransient<ContactSupportViewModel>();
        builder.Services.AddTransient<SignalerProblemeViewModel>();

        // Pages
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<ConnexionPage>();
        builder.Services.AddTransient<InscriptionPage>();
        builder.Services.AddTransient<AccueilPage>();
        builder.Services.AddTransient<ProduitsPage>();
        builder.Services.AddTransient<AjouterProduitPage>();
        builder.Services.AddTransient<NouvelleVentePage>();
        builder.Services.AddTransient<VentePertePage>();
        builder.Services.AddTransient<SuccesPage>();
        builder.Services.AddTransient<HistoriquePage>();
        builder.Services.AddTransient<DetailTransactionPage>();
        builder.Services.AddTransient<ProfilPage>();
        builder.Services.AddTransient<ParametresPage>();
        builder.Services.AddTransient<KiosquePage>();
        builder.Services.AddTransient<ClassementPage>();
        builder.Services.AddTransient<AidePage>();
        builder.Services.AddTransient<AideCategoriePage>();
        builder.Services.AddTransient<AideArticlePage>();
        builder.Services.AddTransient<ContactSupportPage>();
        builder.Services.AddTransient<SignalerProblemePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
