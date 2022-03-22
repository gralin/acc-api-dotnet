namespace Gralin.Avigilon.ControlCenterAPI.DataContracts;

public class LoginResponseContract : IResponseWithStatus
{
    public string Status { get; set; }
    public LoginResultContract Result { get; set; }
}