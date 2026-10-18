using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Domain;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class ContactSupportViewModel : BaseViewModel
{
    private readonly ISupportService _support;
    private readonly IKiosqueService _kiosques;
    private bool _charge;

    public ContactSupportViewModel(ISupportService support, IKiosqueService kiosques)
    {
        _support = support;
        _kiosques = kiosques;
    }

    public ObservableCollection<string> Categories { get; } = new()
    {
        "Produits et stock", "Ventes et historique", "Bénéfices et classements", "Compte et kiosque", "Autre"
    };

    [ObservableProperty] private string nom;
    [ObservableProperty] private string contact;
    [ObservableProperty] private string sujet;
    [ObservableProperty] private string categorie;
    [ObservableProperty] private string description;

    [ObservableProperty] private string erreurNom;
    [ObservableProperty] private string erreurContact;
    [ObservableProperty] private string erreurSujet;
    [ObservableProperty] private string erreurCategorie;
    [ObservableProperty] private string erreurDescription;
    [ObservableProperty] private string messageSucces;

    partial void OnNomChanged(string value) => ErreurNom = null;
    partial void OnContactChanged(string value) => ErreurContact = null;
    partial void OnSujetChanged(string value) => ErreurSujet = null;
    partial void OnCategorieChanged(string value) => ErreurCategorie = null;
    partial void OnDescriptionChanged(string value) => ErreurDescription = null;

    [RelayCommand]
    private async Task ChargerAsync()
    {
        if (_charge) return;
        _charge = true;
        try
        {
            // Pré-remplit les coordonnées depuis le kiosque (modifiables)
            var k = await _kiosques.GetKiosqueAsync();
            Nom = k.Responsable;
            Contact = ValidationHelper.FormaterTelephone(k.Telephone);
        }
        catch
        {
            // Les coordonnées pourront être saisies à la main.
        }
    }

    private bool Valider()
    {
        ErreurNom = string.IsNullOrWhiteSpace(Nom) ? "Indiquez votre nom." : null;
        ErreurContact = string.IsNullOrWhiteSpace(Contact)
            ? "Indiquez un e-mail ou un téléphone pour recevoir la réponse."
            : !ValidationHelper.ContactValide(Contact)
                ? "E-mail ou téléphone invalide (mobile à 9 chiffres commençant par 6)." : null;
        ErreurSujet = string.IsNullOrWhiteSpace(Sujet) || Sujet.Trim().Length < 3 ? "Le sujet est obligatoire." : null;
        ErreurCategorie = string.IsNullOrEmpty(Categorie) ? "Choisissez une catégorie." : null;
        ErreurDescription = string.IsNullOrWhiteSpace(Description) || Description.Trim().Length < 10
            ? "Décrivez votre demande (10 caractères minimum)." : null;

        return ErreurNom == null && ErreurContact == null && ErreurSujet == null
               && ErreurCategorie == null && ErreurDescription == null;
    }

    [RelayCommand]
    private async Task EnvoyerAsync()
    {
        if (IsBusy) return;
        MessageErreur = null;
        MessageSucces = null;

        if (!Valider())
        {
            MessageErreur = "Veuillez corriger les champs signalés.";
            return;
        }

        IsBusy = true;
        try
        {
            var resultat = await _support.EnvoyerDemandeAsync(new DemandeSupport
            {
                Nom = Nom.Trim(), Contact = Contact.Trim(), Sujet = Sujet.Trim(),
                Categorie = Categorie, Description = Description.Trim()
            });

            // Le message dépend de ce qui s'est réellement passé.
            MessageSucces = resultat.Transmis
                ? $"Votre demande {resultat.Reference} a bien été envoyée au support."
                : $"Demande {resultat.Reference} enregistrée sur cet appareil (mode démonstration). Elle n'a PAS été transmise au support.";

            Sujet = null;
            Description = null;
            Categorie = null;
        }
        catch (Exception ex)
        {
            // Les champs sont conservés pour permettre de réessayer.
            MessageErreur = "La demande n'a pas pu être enregistrée : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
