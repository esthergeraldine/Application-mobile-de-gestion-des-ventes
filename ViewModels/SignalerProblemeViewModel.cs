using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class SignalerProblemeViewModel : BaseViewModel
{
    private static readonly Regex FormatReference = new(@"^TRX\d{3,}$", RegexOptions.Compiled);

    private readonly ISupportService _support;
    private readonly IVenteService _ventes;
    private readonly ISelecteurFichierService _fichiers;
    private FichierSelectionne _pieceJointe;

    public SignalerProblemeViewModel(ISupportService support, IVenteService ventes, ISelecteurFichierService fichiers)
    {
        _support = support;
        _ventes = ventes;
        _fichiers = fichiers;
    }

    public ObservableCollection<string> Types { get; } = new()
    {
        "Erreur de calcul (recette, bénéfice)", "Stock incorrect", "Vente ou historique",
        "Affichage ou navigation", "Connexion ou compte", "Autre"
    };

    public DateTime DateMax => DateTime.Today;

    [ObservableProperty] private string type;
    [ObservableProperty] private string description;
    [ObservableProperty] private DateTime dateProbleme = DateTime.Today;
    [ObservableProperty] private string referenceTransaction;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(APieceJointe))]
    private string pieceJointeNom;

    [ObservableProperty] private string erreurType;
    [ObservableProperty] private string erreurDescription;
    [ObservableProperty] private string erreurDate;
    [ObservableProperty] private string erreurReference;
    [ObservableProperty] private string messageSucces;

    public bool APieceJointe => !string.IsNullOrEmpty(PieceJointeNom);

    partial void OnTypeChanged(string value) => ErreurType = null;
    partial void OnDescriptionChanged(string value) => ErreurDescription = null;
    partial void OnDateProblemeChanged(DateTime value) => ErreurDate = null;
    partial void OnReferenceTransactionChanged(string value) => ErreurReference = null;

    [RelayCommand]
    private async Task ChoisirPieceJointeAsync()
    {
        try
        {
            var fichier = await _fichiers.ChoisirImageAsync();
            if (fichier == null) return; // annulé par l'utilisateur
            _pieceJointe = fichier;
            PieceJointeNom = fichier.Nom;
        }
        catch (Exception ex)
        {
            MessageErreur = "Impossible de sélectionner le fichier : " + ex.Message;
        }
    }

    [RelayCommand]
    private void RetirerPieceJointe()
    {
        _pieceJointe = null;
        PieceJointeNom = null;
    }

    private async Task<bool> ValiderAsync()
    {
        ErreurType = string.IsNullOrEmpty(Type) ? "Choisissez le type de problème." : null;
        ErreurDescription = string.IsNullOrWhiteSpace(Description) || Description.Trim().Length < 10
            ? "Décrivez le problème (10 caractères minimum)." : null;
        ErreurDate = DateProbleme.Date > DateTime.Today ? "La date ne peut pas être dans le futur." : null;

        ErreurReference = null;
        var reference = (ReferenceTransaction ?? string.Empty).Trim().ToUpperInvariant();
        if (reference.Length > 0)
        {
            if (!FormatReference.IsMatch(reference))
                ErreurReference = "Format attendu : TRX suivi de chiffres (ex. TRX002).";
            else if (await _ventes.GetVenteAsync(reference) == null)
                ErreurReference = "Aucune transaction ne porte cette référence.";
        }

        return ErreurType == null && ErreurDescription == null && ErreurDate == null && ErreurReference == null;
    }

    [RelayCommand]
    private async Task EnvoyerAsync()
    {
        if (IsBusy) return;
        MessageErreur = null;
        MessageSucces = null;

        IsBusy = true;
        try
        {
            if (!await ValiderAsync())
            {
                MessageErreur = "Veuillez corriger les champs signalés.";
                return;
            }

            var reference = (ReferenceTransaction ?? string.Empty).Trim().ToUpperInvariant();
            var resultat = await _support.EnvoyerSignalementAsync(new SignalementProbleme
            {
                Type = Type,
                Description = Description.Trim(),
                DateProbleme = DateProbleme.Date,
                ReferenceTransaction = reference.Length == 0 ? null : reference,
                PieceJointeNom = _pieceJointe?.Nom,
                PieceJointeChemin = _pieceJointe?.Chemin
            });

            MessageSucces = resultat.Transmis
                ? $"Votre signalement {resultat.Reference} a bien été envoyé."
                : $"Signalement {resultat.Reference} enregistré sur cet appareil (mode démonstration). Il n'a PAS été transmis au support.";

            Type = null;
            Description = null;
            ReferenceTransaction = null;
            DateProbleme = DateTime.Today;
            RetirerPieceJointe();
        }
        catch (Exception ex)
        {
            MessageErreur = "Le signalement n'a pas pu être enregistré : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
