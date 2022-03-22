namespace Gralin.Avigilon.ControlCenterAPI.DataContracts;

public class CameraResponseContract : IResponseWithStatus
{
    public string Status { get; set; }
    public CamerasContract Result { get; set; }
}