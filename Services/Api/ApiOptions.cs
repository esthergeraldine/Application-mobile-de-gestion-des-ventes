namespace VenteJour.Services.Api;

public class ApiOptions
{
    /// <summary>
    /// True tant que l'API est simulée par FakeApiHandler : les envois ne sont alors PAS réellement transmis
    /// à un serveur, et l'interface doit l'indiquer. Passer à false avec la vraie API.
    /// </summary>
    public bool Simulation { get; set; } = true;
}
