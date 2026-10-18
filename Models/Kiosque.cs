namespace VenteJour.Models;

public class Kiosque
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;

    /// <summary>Numéro stocké sous forme de 9 chiffres (ex. 699123456).</summary>
    public string Telephone { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string Quartier { get; set; } = string.Empty;
    public string AdressePrecise { get; set; } = string.Empty;
}
