namespace VenteJour.Services;

public interface IParametresService
{
    bool AlertesVenteAPerte { get; set; }
    bool RappelsCloture { get; set; }
    bool ThemeSombre { get; set; }
    string Langue { get; set; }
    void AppliquerTheme();
}
