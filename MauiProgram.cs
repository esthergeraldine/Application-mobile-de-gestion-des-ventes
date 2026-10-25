using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using VenteJour.Services;
using VenteJour.Services.Api;
using VenteJour.Services.Fake;
using VenteJour.Services.Json;
using VenteJour.Services.Sqlite;
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
        // PHASE C : base SQLite, UNE PAR COMPTE (ventejour-<id du compte>.db3)
        //
        // Comptes locaux : comptes.json (empreintes PBKDF2, jamais de mot de passe en clair).
        // Chaque compte a sa propre base : un compte neuf démarre à zéro, sans données de démonstration.
        // Les services métier, les ViewModels et les pages ne connaissent que les interfaces.
        // ------------------------------------------------------------------
        var dossierDonnees = FileSystem.AppDataDirectory;

        builder.Services.AddSingleton<ISession, SessionUtilisateur>();
        builder.Services.AddSingleton<IStockageSession, PreferencesStockageSession>();
        builder.Services.AddSingleton<ICompteRepository>(_ => new JsonCompteRepository(dossierDonnees));
        builder.Services.AddSingleton<IAuthService, AuthService>();

        builder.Services.AddSingleton(sp =>
        {
            var session = sp.GetRequiredService<ISession>();
            return new SqliteBaseDeDonnees(() =>
            {
                var compteId = session.Courant?.Id ?? throw new InvalidOperationException("Aucun utilisateur connecté.");
                return Path.Combine(dossierDonnees, $"ventejour-{compteId}.db3");
            });
        });

        builder.Services.AddSingleton<IProduitRepository, SqliteProduitRepository>();
        builder.Services.AddSingleton<IVenteRepository, SqliteVenteRepository>();
        builder.Services.AddSingleton<IKiosqueRepository, SqliteKiosqueRepository>();
        builder.Services.AddSingleton<IDonneesDemoService, DonneesDemoService>();

        builder.Services.AddSingleton<IProduitService, ProduitService>();
        builder.Services.AddSingleton<IVenteService, VenteService>();
        builder.Services.AddSingleton<IBilanService, BilanService>();
        builder.Services.AddSingleton<IClassementService, ClassementService>();
        builder.Services.AddSingleton<IKiosqueService, KiosqueService>();

        // Contenus locaux et API simulée (phase B)
        builder.Services.AddSingleton<ILocalisationService, FakeLocalisationService>();
        builder.Services.AddSingleton<IAideService, FakeAideService>();

        // ------------------------------------------------------------------
        // PHASE B : consommer une API (simulée, hors ligne)
        //
        // HttpClient est fourni par injection de dépendances. Ici il est branché sur un FAUX serveur (FakeApiHandler).
        //   Vraie API plus tard : remplacer la ligne HttpClient par
        //       new HttpClient { BaseAddress = new Uri("https://votre-api.exemple.com/") }
        //   et mettre Simulation = false. Aucun ViewModel ni aucune page ne change.
        // ------------------------------------------------------------------
        builder.Services.AddSingleton(new ApiOptions { Simulation = true });
        builder.Services.AddSingleton(new HttpClient(new FakeApiHandler())
        {
            BaseAddress = new Uri("https://api.ventejour.exemple/")
        });
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<ICacheLocal, PreferencesCache>();
        builder.Services.AddSingleton<ITauxChangeService, TauxChangeService>();
        builder.Services.AddSingleton<ISupportService, ApiSupportService>();

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
        builder.Services.AddTransient<MotDePasseOublieViewModel>();
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
        builder.Services.AddTransient<MotDePasseOubliePage>();
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
