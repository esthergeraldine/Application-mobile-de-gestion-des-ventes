using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Domain;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class KiosqueViewModel : BaseViewModel
{
    private readonly IKiosqueService _kiosques;
    private readonly ILocalisationService _lieux;
    private readonly IDialogService _dialog;

    private Kiosque _initial;
    private bool _init;
    private bool _charge;

    public KiosqueViewModel(IKiosqueService kiosques, ILocalisationService lieux, IDialogService dialog)
    {
        _kiosques = kiosques;
        _lieux = lieux;
        _dialog = dialog;
    }

    public ObservableCollection<string> Regions { get; } = new();
    public ObservableCollection<string> Villes { get; } = new();
    public ObservableCollection<string> Quartiers { get; } = new();

    [ObservableProperty] private string nom;
    [ObservableProperty] private string responsable;
    [ObservableProperty] private string telephone;
    [ObservableProperty] private string adressePrecise;
    [ObservableProperty] private string region;
    [ObservableProperty] private string ville;
    [ObservableProperty] private string quartier;

    [ObservableProperty] private string erreurNom;
    [ObservableProperty] private string erreurResponsable;
    [ObservableProperty] private string erreurTelephone;
    [ObservableProperty] private string erreurRegion;
    [ObservableProperty] private string erreurVille;
    [ObservableProperty] private string erreurQuartier;
    [ObservableProperty] private string messageSucces;

    public bool EstChargee => _charge;

    public string Initiales
    {
        get
        {
            var mots = (Nom ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return mots.Length == 0 ? "?" : string.Concat(mots.Take(2).Select(m => char.ToUpperInvariant(m[0])));
        }
    }

    public bool ADesModifications
    {
        get
        {
            if (_initial == null) return false;
            static string T(string s) => (s ?? string.Empty).Trim();
            return T(Nom) != T(_initial.Nom)
                   || T(Responsable) != T(_initial.Responsable)
                   || ValidationHelper.ChiffresTelephone(Telephone) != ValidationHelper.ChiffresTelephone(_initial.Telephone)
                   || T(Region) != T(_initial.Region)
                   || T(Ville) != T(_initial.Ville)
                   || T(Quartier) != T(_initial.Quartier)
                   || T(AdressePrecise) != T(_initial.AdressePrecise);
        }
    }

    private void Modifie()
    {
        MessageSucces = null;
        OnPropertyChanged(nameof(ADesModifications));
        OnPropertyChanged(nameof(Initiales));
    }

    partial void OnNomChanged(string value) { ErreurNom = null; Modifie(); }
    partial void OnResponsableChanged(string value) { ErreurResponsable = null; Modifie(); }
    partial void OnTelephoneChanged(string value) { ErreurTelephone = null; Modifie(); }
    partial void OnAdressePreciseChanged(string value) => Modifie();

    partial void OnRegionChanged(string value)
    {
        ErreurRegion = null;
        Modifie();
        if (_init) return;

        // Une nouvelle région invalide la ville et le quartier choisis
        Ville = null;
        Quartier = null;
        Remplir(Villes, null);
        Remplir(Quartiers, null);
        _ = ChargerVillesAsync(value);
    }

    partial void OnVilleChanged(string value)
    {
        ErreurVille = null;
        Modifie();
        if (_init) return;

        Quartier = null;
        Remplir(Quartiers, null);
        _ = ChargerQuartiersAsync(Region, value);
    }

    partial void OnQuartierChanged(string value) { ErreurQuartier = null; Modifie(); }

    private static void Remplir(ObservableCollection<string> cible, IEnumerable<string> source)
    {
        cible.Clear();
        if (source == null) return;
        foreach (var s in source)
            cible.Add(s);
    }

    private async Task ChargerVillesAsync(string regionChoisie)
    {
        if (string.IsNullOrEmpty(regionChoisie)) return;
        try
        {
            var villes = await _lieux.GetVillesAsync(regionChoisie);
            if (Region == regionChoisie) // ignore une réponse devenue obsolète
                Remplir(Villes, villes);
        }
        catch (Exception ex)
        {
            MessageErreur = "Impossible de charger les villes : " + ex.Message;
        }
    }

    private async Task ChargerQuartiersAsync(string regionChoisie, string villeChoisie)
    {
        if (string.IsNullOrEmpty(regionChoisie) || string.IsNullOrEmpty(villeChoisie)) return;
        try
        {
            var quartiers = await _lieux.GetQuartiersAsync(regionChoisie, villeChoisie);
            if (Region == regionChoisie && Ville == villeChoisie)
                Remplir(Quartiers, quartiers);
        }
        catch (Exception ex)
        {
            MessageErreur = "Impossible de charger les quartiers : " + ex.Message;
        }
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        if (_charge) return; // ne jamais écraser une saisie en cours
        IsBusy = true;
        MessageErreur = null;
        try
        {
            var k = await _kiosques.GetKiosqueAsync();
            var regions = await _lieux.GetRegionsAsync();
            var villes = await _lieux.GetVillesAsync(k.Region);
            var quartiers = await _lieux.GetQuartiersAsync(k.Region, k.Ville);

            _init = true;
            Remplir(Regions, regions);
            Remplir(Villes, villes);
            Remplir(Quartiers, quartiers);
            Nom = k.Nom;
            Responsable = k.Responsable;
            Telephone = ValidationHelper.FormaterTelephone(k.Telephone);
            AdressePrecise = k.AdressePrecise;
            Region = k.Region;
            Ville = k.Ville;
            Quartier = k.Quartier;
            _init = false;

            _initial = Copier(k);
            _charge = true;
            OnPropertyChanged(nameof(EstChargee));
            Modifie();
        }
        catch (Exception ex)
        {
            MessageErreur = "Impossible de charger les informations du kiosque : " + ex.Message;
        }
        finally
        {
            _init = false;
            IsBusy = false;
        }
    }

    private bool Valider()
    {
        ErreurNom = string.IsNullOrWhiteSpace(Nom) || Nom.Trim().Length < 2
            ? "Le nom du kiosque est obligatoire (2 caractères minimum)." : null;
        ErreurResponsable = string.IsNullOrWhiteSpace(Responsable) || Responsable.Trim().Length < 2
            ? "Le nom du responsable est obligatoire." : null;
        ErreurTelephone = string.IsNullOrWhiteSpace(Telephone)
            ? "Le numéro de téléphone est obligatoire."
            : !ValidationHelper.TelephoneValide(Telephone)
                ? "Numéro invalide : saisissez 9 chiffres commençant par 6 (ex. 6 99 12 34 56)." : null;
        ErreurRegion = string.IsNullOrEmpty(Region) ? "Choisissez une région." : null;
        ErreurVille = string.IsNullOrEmpty(Ville) ? "Choisissez une ville." : null;
        ErreurQuartier = string.IsNullOrEmpty(Quartier) ? "Choisissez un quartier." : null;

        return ErreurNom == null && ErreurResponsable == null && ErreurTelephone == null
               && ErreurRegion == null && ErreurVille == null && ErreurQuartier == null;
    }

    [RelayCommand]
    private async Task EnregistrerAsync()
    {
        if (IsBusy) return;
        MessageErreur = null;
        MessageSucces = null;

        // En cas d'erreur de validation, rien n'est effacé : les champs gardent leur contenu.
        if (!Valider())
        {
            MessageErreur = "Veuillez corriger les champs signalés.";
            return;
        }

        IsBusy = true;
        try
        {
            var k = new Kiosque
            {
                Id = _initial?.Id ?? 0,
                Nom = Nom.Trim(),
                Responsable = Responsable.Trim(),
                Telephone = ValidationHelper.ChiffresTelephone(Telephone),
                Region = Region,
                Ville = Ville,
                Quartier = Quartier,
                AdressePrecise = (AdressePrecise ?? string.Empty).Trim()
            };

            await _kiosques.EnregistrerAsync(k);

            _initial = Copier(k);
            Telephone = ValidationHelper.FormaterTelephone(k.Telephone);
            OnPropertyChanged(nameof(ADesModifications));

            // Message honnête : on ne dit "serveur" que si un serveur est réellement contacté.
            MessageSucces = _kiosques.EstSimulation
                ? "Modifications enregistrées sur cet appareil (mode démonstration : aucune donnée envoyée à un serveur)."
                : "Modifications enregistrées.";
        }
        catch (Exception ex)
        {
            MessageErreur = "L'enregistrement a échoué : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Appelé avant de quitter la page. Retourne true si on peut quitter.</summary>
    public async Task<bool> ConfirmerAbandonAsync()
    {
        if (!ADesModifications) return true;
        return await _dialog.ConfirmAsync("Modifications non enregistrées",
            "Si vous quittez cette page, vos modifications seront perdues. Quitter quand même ?",
            "Quitter", "Rester");
    }

    private static Kiosque Copier(Kiosque k) => new()
    {
        Id = k.Id, Nom = k.Nom, Responsable = k.Responsable, Telephone = k.Telephone,
        Region = k.Region, Ville = k.Ville, Quartier = k.Quartier, AdressePrecise = k.AdressePrecise
    };
}
