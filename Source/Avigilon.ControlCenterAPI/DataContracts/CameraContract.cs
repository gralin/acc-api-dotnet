namespace Gralin.Avigilon.ControlCenterAPI.DataContracts;

public class CamerasContract
{
    public List<CameraContract> Cameras { get; set; }
}

public class CameraContract
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Available { get; set; }
    public bool Connected { get; set; }
    public string ConnectionState { get; set; }
    public bool Active { get; set; }
    public uint LogicalId { get; set; }
    public ConnectionStatusContract ConnectionStatus { get; set; }

    public class ConnectionStatusContract
    {
        public bool IsConnectable { get; set; }
        public string State { get; set; }
    }
}