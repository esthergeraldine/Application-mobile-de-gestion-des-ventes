namespace VenteJour.Views.Controls;

public partial class BackHeader : ContentView
{
    public static readonly BindableProperty TitreProperty = BindableProperty.Create(
        nameof(Titre), typeof(string), typeof(BackHeader), string.Empty,
        propertyChanged: (b, o, n) => ((BackHeader)b).TitreLabel.Text = (string)n);

    public static readonly BindableProperty RouteRetourProperty = BindableProperty.Create(
        nameof(RouteRetour), typeof(string), typeof(BackHeader), "..");

    public BackHeader()
    {
        InitializeComponent();
    }

    public string Titre
    {
        get => (string)GetValue(TitreProperty);
        set => SetValue(TitreProperty, value);
    }

    /// <summary>".." par défaut (page précédente) ou une route absolue comme "//accueil".</summary>
    public string RouteRetour
    {
        get => (string)GetValue(RouteRetourProperty);
        set => SetValue(RouteRetourProperty, value);
    }

    private async void OnBack(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(RouteRetour);
}
