using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Helpers;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class DetailTransactionViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IVenteService _ventes;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;
    private string _id;

    public DetailTransactionViewModel(IVenteService ventes, INavigationService nav, IDialogService dialog)
    {
        _ventes = ventes;
        _nav = nav;
        _dialog = dialog;
    }

    [ObservableProperty] private Vente vente;

    partial void OnVenteChanged(Vente value)
    {
        OnPropertyChanged(nameof(DateTexte));
        OnPropertyChanged(nameof(MargeTexte));
        OnPropertyChanged(nameof(PeutAnnuler));
        OnPropertyChanged(nameof(StatutCouleur));
    }

    public bool PeutAnnuler => Vente != null && Vente.EstValide;
    public Color StatutCouleur => Vente != null && !Vente.EstValide ? Color.FromArgb("#D32F2F") : Color.FromArgb("#006652");

    public string DateTexte => Vente == null ? string.Empty : Vente.Date.ToString("dd/MM/yyyy - HH:mm", CultureInfo.InvariantCulture);

    public string MargeTexte => Vente == null
        ? string.Empty
        : $"{Format.Fcfa(Vente.Benefice)} ({Format.Pourcentage(Vente.MargePourcentage)}%)";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var id) && id != null)
            _id = Convert.ToString(id, CultureInfo.InvariantCulture);
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        if (!string.IsNullOrEmpty(_id))
            Vente = await _ventes.GetVenteAsync(_id);
    }

    [RelayCommand]
    private async Task AnnulerAsync()
    {
        if (Vente == null || !Vente.EstValide || IsBusy) return;

        var confirme = await _dialog.ConfirmAsync("Annuler la vente",
            $"Annuler la transaction {Vente.Id} ? Les articles seront remis en stock et la vente ne sera plus comptabilisée. Cette action est définitive.",
            "Annuler la vente", "Garder");
        if (!confirme) return;

        IsBusy = true;
        MessageErreur = null;
        try
        {
            await _ventes.AnnulerVenteAsync(Vente.Id);
            Vente = await _ventes.GetVenteAsync(Vente.Id);
        }
        catch (Exception ex)
        {
            MessageErreur = "L'annulation a échoué : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AjouterAsync() => _nav.GoToAsync(Routes.NouvelleVente);
}
