using VenteJour.Models;

namespace VenteJour.Services;

public class SelecteurFichierService : ISelecteurFichierService
{
    public async Task<FichierSelectionne> ChoisirImageAsync()
    {
        var resultat = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Choisir une capture d'écran",
            FileTypes = FilePickerFileType.Images
        });

        return resultat == null ? null : new FichierSelectionne { Nom = resultat.FileName, Chemin = resultat.FullPath };
    }
}
