using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VenteJour.Models;
using VenteJour.Services;

namespace VenteJour.ViewModels;

public partial class AideViewModel : BaseViewModel
{
    private readonly IAideService _aide;
    private readonly ISupportService _support;
    private readonly INavigationService _nav;
    private int _version;

    public AideViewModel(IAideService aide, ISupportService support, INavigationService nav)
    {
        _aide = aide;
        _support = support;
        _nav = nav;
    }

    public ObservableCollection<CategorieAide> Categories { get; } = new();
    public ObservableCollection<ArticleAide> Resultats { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstEnRecherche))]
    [NotifyPropertyChangedFor(nameof(PasEnRecherche))]
    private string recherche;

    [ObservableProperty] private bool aucunResultat;
    [ObservableProperty] private string resumeRecherche;

    public bool EstEnRecherche => !string.IsNullOrWhiteSpace(Recherche);
    public bool PasEnRecherche => !EstEnRecherche;

    // Texte honnête : la promesse de délai n'apparaît que si les demandes sont réellement transmises.
    public string NoteSupport => _support.EstSimulation
        ? "Mode démonstration : les demandes sont enregistrées sur cet appareil mais ne sont pas transmises à un serveur."
        : "Réponse du support sous 24 à 48 h.";

    partial void OnRechercheChanged(string value) => _ = RechercherAsync(value);

    private async Task RechercherAsync(string texte)
    {
        int version = ++_version;
        if (string.IsNullOrWhiteSpace(texte))
        {
            Resultats.Clear();
            AucunResultat = false;
            ResumeRecherche = null;
            return;
        }

        try
        {
            var trouves = await _aide.RechercherAsync(texte);
            if (version != _version) return; // une saisie plus récente a pris le relais

            Resultats.Clear();
            foreach (var a in trouves)
                Resultats.Add(a);
            AucunResultat = trouves.Count == 0;
            ResumeRecherche = trouves.Count == 0 ? null : $"{trouves.Count} résultat{(trouves.Count > 1 ? "s" : "")}";
        }
        catch (Exception ex)
        {
            MessageErreur = "La recherche a échoué : " + ex.Message;
        }
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        MessageErreur = null;
        try
        {
            var categories = await _aide.GetCategoriesAsync();
            Categories.Clear();
            foreach (var c in categories)
                Categories.Add(c);
        }
        catch (Exception ex)
        {
            MessageErreur = "Impossible de charger l'aide : " + ex.Message;
        }
    }

    [RelayCommand]
    private void EffacerRecherche() => Recherche = string.Empty;

    [RelayCommand]
    private Task OuvrirCategorieAsync(CategorieAide categorie)
        => categorie == null ? Task.CompletedTask : _nav.GoToAsync($"{Routes.AideCategorie}?id={categorie.Id}");

    [RelayCommand]
    private Task OuvrirArticleAsync(ArticleAide article)
        => article == null ? Task.CompletedTask : _nav.GoToAsync($"{Routes.AideArticle}?id={article.Id}");

    [RelayCommand]
    private Task ContacterAsync() => _nav.GoToAsync(Routes.ContactSupport);

    [RelayCommand]
    private Task SignalerAsync() => _nav.GoToAsync(Routes.SignalerProbleme);
}
