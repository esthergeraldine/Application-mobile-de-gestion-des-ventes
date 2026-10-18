using CommunityToolkit.Mvvm.ComponentModel;

namespace VenteJour.ViewModels;

/// <summary>Pastille de filtre (Tous / Boissons ... ou Aujourd'hui / Hier / Semaine).</summary>
public partial class ChipItem : ObservableObject
{
    private static readonly Color Vert = Color.FromArgb("#006652");
    private static readonly Color Menthe = Color.FromArgb("#E3F4EE");

    /// <summary>Clé fonctionnelle (null = "Tous").</summary>
    public string Cle { get; set; }

    [ObservableProperty]
    private string libelle;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Fond))]
    [NotifyPropertyChangedFor(nameof(Texte))]
    private bool estActive;

    public Color Fond => EstActive ? Vert : Menthe;
    public Color Texte => EstActive ? Colors.White : Vert;
}
