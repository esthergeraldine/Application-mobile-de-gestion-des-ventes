using VenteJour.ViewModels;

namespace VenteJour.Views;

public partial class KiosquePage : ContentPage
{
    private readonly KiosqueViewModel _vm;
    private bool _abonne;

    public KiosquePage(KiosqueViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_abonne)
        {
            Shell.Current.Navigating += OnNavigating;
            _abonne = true;
        }
        await _vm.ChargerCommand.ExecuteAsync(null); // ne recharge pas si les données sont déjà chargées
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_abonne)
        {
            Shell.Current.Navigating -= OnNavigating;
            _abonne = false;
        }
    }

    // Intercepte TOUTE sortie de la page (bouton retour, geste retour Android, barre du bas...)
    // tant que des modifications ne sont pas enregistrées.
    private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        if (!_vm.ADesModifications || !e.CanCancel)
            return;

        var deferral = e.GetDeferral();
        try
        {
            if (!await _vm.ConfirmerAbandonAsync())
                e.Cancel();
        }
        finally
        {
            deferral.Complete();
        }
    }
}
