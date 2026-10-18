namespace VenteJour.Models;

public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public int PrixAchat { get; set; }
    public int PrixVente { get; set; }
    public int Stock { get; set; }

    /// <summary>Clé d'icône : drop, cookie, cup, bread, box.</summary>
    public string Icone { get; set; } = "drop";

    public int BeneficeUnitaire => PrixVente - PrixAchat;
    public bool EstVenteAPerte => PrixVente < PrixAchat;
}
