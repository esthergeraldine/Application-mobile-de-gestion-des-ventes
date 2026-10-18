namespace VenteJour.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(2200);
        await Shell.Current.GoToAsync("//connexion");
    }
}
