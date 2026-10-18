namespace VenteJour.Views.Controls;

/// <summary>Barre de navigation inférieure (Accueil, Produits, Vente, Historique, Profil).</summary>
public partial class BottomNav : ContentView
{
    private static readonly Color Menthe = Color.FromArgb("#E3F4EE");

    public static readonly BindableProperty ActifProperty = BindableProperty.Create(
        nameof(Actif), typeof(string), typeof(BottomNav), string.Empty,
        propertyChanged: (b, o, n) => ((BottomNav)b).Rafraichir());

    public BottomNav()
    {
        InitializeComponent();
    }

    /// <summary>accueil | produits | historique | profil</summary>
    public string Actif
    {
        get => (string)GetValue(ActifProperty);
        set => SetValue(ActifProperty, value);
    }

    private void Rafraichir()
    {
        Appliquer(PillAccueil, LblAccueil, Actif == "accueil");
        Appliquer(PillProduits, LblProduits, Actif == "produits");
        Appliquer(PillHistorique, LblHistorique, Actif == "historique");
        Appliquer(PillProfil, LblProfil, Actif == "profil");
    }

    private static void Appliquer(Border pastille, Label libelle, bool actif)
    {
        pastille.BackgroundColor = actif ? Menthe : Colors.Transparent;
        libelle.FontAttributes = actif ? FontAttributes.Bold : FontAttributes.None;
    }

    private static Task Aller(string route) => Shell.Current.GoToAsync(route);

    private async void OnAccueil(object sender, TappedEventArgs e) => await Aller("//accueil");
    private async void OnProduits(object sender, TappedEventArgs e) => await Aller("//produits");
    private async void OnVente(object sender, TappedEventArgs e) => await Aller("nouvellevente");
    private async void OnHistorique(object sender, TappedEventArgs e) => await Aller("//historique");
    private async void OnProfil(object sender, TappedEventArgs e) => await Aller("//profil");
}
