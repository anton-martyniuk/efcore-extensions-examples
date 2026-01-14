namespace Domain.Devices;

public class Device
{
    public long DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string FirmwareVersion { get; set; } = string.Empty;
    public string HardwareVersion { get; set; } = string.Empty;
    public DeviceStatus Status { get; set; }
    public DateTime LastSeenAt { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string? Configuration { get; set; } = string.Empty;
    public List<Component> Components { get; set; } = [];
}
