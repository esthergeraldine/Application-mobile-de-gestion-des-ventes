using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Helpers;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public class ClassementItem
{
    public int Rang { get; set; }
    public string Nom { get; set; }
    public string Icone { get; set; }
    public string QuantiteTexte { get; set; }
    public string ValeurPrincipaleLibelle { get; set; }
    public string ValeurPrincipaleTexte { get; set; }
    public string ValeurSecondaireTexte { get; set; }
    public Color CouleurValeur { get; set; }
    public Color CouleurIndicateur { get; set; }
    public double PartRelative { get; set; }
    public bool EstPremier { get; set; }
}

public partial class ClassementViewModel : BaseViewModel
{
    private static readonly Color Vert = Color.FromArgb("#006652");
    private static readonly Color Orange = Color.FromArgb("#F28C28");
    private static readonly Color Rouge = Color.FromArgb("#D32F2F");

    private readonly IClassementService _classement;
    private CritereClassement _critere = CritereClassement.Recettes;
    private PeriodeClassement _periode = PeriodeClassement.Aujourdhui;
    private int _version;

    public ClassementViewModel(IClassementService classement)
    {
        _classement = classement;

        Criteres.Add(new ChipItem { Cle = nameof(CritereClassement.Recettes), Libelle = "Par recettes", EstActive = true });
        Criteres.Add(new ChipItem { Cle = nameof(CritereClassement.Benefices), Libelle = "Par bénéfices" });

        Periodes.Add(new ChipItem { Cle = nameof(PeriodeClassement.Aujourdhui), Libelle = "Aujourd'hui", EstActive = true });
        Periodes.Add(new ChipItem { Cle = nameof(PeriodeClassement.Semaine), Libelle = "Cette semaine" });
        Periodes.Add(new ChipItem { Cle = nameof(PeriodeClassement.Mois), Libelle = "Ce mois" });
        Periodes.Add(new ChipItem { Cle = nameof(PeriodeClassement.Personnalisee), Libelle = "Personnalisée" });
    }

    public ObservableCollection<ChipItem> Criteres { get; } = new();
    public ObservableCollection<ChipItem> Periodes { get; } = new();
    public ObservableCollection<ClassementItem> Items { get; } = new();

    public DateTime DateMax => DateTime.Today;

    [ObservableProperty] private DateTime dateDebut = DateTime.Today.AddDays(-6);
    [ObservableProperty] private DateTime dateFin = DateTime.Today;
    [ObservableProperty] private bool estEnChargement;
    [ObservableProperty] private bool estVide;
    [ObservableProperty] private string periodeTexte;

    public bool EstPersonnalisee => _periode == PeriodeClassement.Personnalisee;
    public bool AfficheListe => !EstEnChargement && !ADesErreur && !EstVide;

    partial void OnEstEnChargementChanged(bool value) => OnPropertyChanged(nameof(AfficheListe));
    partial void OnEstVideChanged(bool value) => OnPropertyChanged(nameof(AfficheListe));

    partial void OnDateDebutChanged(DateTime value)
    {
        if (EstPersonnalisee) _ = ChargerAsync();
    }

    partial void OnDateFinChanged(DateTime value)
    {
        if (EstPersonnalisee) _ = ChargerAsync();
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        int version = ++_version;
        EstEnChargement = true;
        MessageErreur = null;
        OnPropertyChanged(nameof(AfficheListe));

        try
        {
            var plage = PlageDates.Pour(_periode, DateTime.Now, DateDebut, DateFin);
            PeriodeTexte = plage.Fin - plage.Debut <= TimeSpan.FromDays(1)
                ? plage.Debut.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                : $"{plage.Debut:dd/MM/yyyy} – {plage.Fin.AddDays(-1):dd/MM/yyyy}";

            // Toujours recalculé à partir des ventes courantes : jamais de classement mis en cache.
            var liste = await _classement.GetClassementAsync(_critere, plage);
            if (version != _version) return; // une demande plus récente est en cours

            Items.Clear();
            foreach (var c in liste)
                Items.Add(Convertir(c));
            EstVide = Items.Count == 0;
        }
        catch (Exception ex)
        {
            if (version != _version) return;
            Items.Clear();
            EstVide = false;
            MessageErreur = "Impossible de calculer le classement : " + ex.Message;
        }
        finally
        {
            if (version == _version)
                EstEnChargement = false;
            OnPropertyChanged(nameof(AfficheListe));
        }
    }

    private ClassementItem Convertir(ClassementProduit c)
    {
        bool parRecettes = _critere == CritereClassement.Recettes;
        int valeur = c.Valeur;
        int secondaire = parRecettes ? c.Benefice : c.Recette;

        return new ClassementItem
        {
            Rang = c.Rang,
            Nom = c.Nom,
            Icone = c.Icone,
            QuantiteTexte = $"{Format.Nombre(c.QuantiteVendue)} vendu{(c.QuantiteVendue > 1 ? "s" : string.Empty)}",
            ValeurPrincipaleLibelle = parRecettes ? "Recette" : "Bénéfice",
            ValeurPrincipaleTexte = Format.Fcfa(valeur),
            ValeurSecondaireTexte = (parRecettes ? "Bénéfice : " : "Recette : ") + Format.Fcfa(secondaire),
            CouleurValeur = valeur < 0 ? Rouge : Vert,
            CouleurIndicateur = c.Rang == 1 ? Orange : valeur < 0 ? Rouge : Vert,
            PartRelative = c.PartRelative,
            EstPremier = c.Rang == 1
        };
    }

    [RelayCommand]
    private async Task SelectionnerCritereAsync(ChipItem chip)
    {
        if (chip == null) return;
        _critere = Enum.Parse<CritereClassement>(chip.Cle);
        foreach (var c in Criteres)
            c.EstActive = c == chip;
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task SelectionnerPeriodeAsync(ChipItem chip)
    {
        if (chip == null) return;
        _periode = Enum.Parse<PeriodeClassement>(chip.Cle);
        foreach (var c in Periodes)
            c.EstActive = c == chip;
        OnPropertyChanged(nameof(EstPersonnalisee));
        await ChargerAsync();
    }
}
