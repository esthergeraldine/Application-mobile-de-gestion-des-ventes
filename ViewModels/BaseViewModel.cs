using CommunityToolkit.Mvvm.ComponentModel;

namespace VenteJour.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasOccupe))]
    private bool isBusy;

    public bool PasOccupe => !IsBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ADesErreur))]
    private string messageErreur;

    public bool ADesErreur => !string.IsNullOrEmpty(MessageErreur);
}
