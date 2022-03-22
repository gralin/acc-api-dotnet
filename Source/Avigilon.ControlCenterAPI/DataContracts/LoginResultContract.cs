namespace Gralin.Avigilon.ControlCenterAPI.DataContracts;

public class LoginResultContract
{
    public string Session { get; set; }
    public string ExternalUserId { get; set; }
    public string DomainId { get; set; }
}