namespace VenteJour.Services;

public interface INavigationService
{
    Task GoToAsync(string route);
    Task GoBackAsync();
}

public interface IDialogService
{
    Task AlertAsync(string titre, string message, string ok = "OK");
    Task<bool> ConfirmAsync(string titre, string message, string oui = "Oui", string non = "Non");
}

public class ShellNavigationService : INavigationService
{
    public Task GoToAsync(string route) => Shell.Current.GoToAsync(route);
    public Task GoBackAsync() => Shell.Current.GoToAsync("..");
}

public class DialogService : IDialogService
{
    public Task AlertAsync(string titre, string message, string ok = "OK")
        => Shell.Current.DisplayAlertAsync(titre, message, ok);

    public Task<bool> ConfirmAsync(string titre, string message, string oui = "Oui", string non = "Non")
        => Shell.Current.DisplayAlertAsync(titre, message, oui, non);
}
