namespace Gralin.Avigilon.ControlCenterAPI.DataContracts;

public class LoginRequestContract
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ClientName { get; set; }
    public string AuthorizationToken { get; set; }
}