namespace VenteJour.Views.Controls;

public partial class AppHeader : ContentView
{
    public static readonly BindableProperty SousTitreProperty = BindableProperty.Create(
        nameof(SousTitre), typeof(string), typeof(AppHeader), string.Empty,
        propertyChanged: (b, o, n) =>
        {
            var header = (AppHeader)b;
            var texte = (string)n;
            header.SousTitreLabel.Text = texte;
            header.SousTitreLabel.IsVisible = !string.IsNullOrEmpty(texte);
        });

    public AppHeader()
    {
        InitializeComponent();
    }

    public string SousTitre
    {
        get => (string)GetValue(SousTitreProperty);
        set => SetValue(SousTitreProperty, value);
    }

    private async void OnNotifications(object sender, TappedEventArgs e)
        => await Shell.Current.DisplayAlertAsync("Notifications", "Aucune nouvelle notification.", "OK");

    private async void OnProfil(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//profil");
}
