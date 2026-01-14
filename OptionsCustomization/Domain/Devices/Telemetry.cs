namespace Domain.Devices;

public class Telemetry
{
    public Guid TelemetryId { get; set; }
    public long DeviceId { get; set; }
    public Guid ComponentId { get; set; }
    public double Value { get; set; }
    public TelemetryQuality Quality { get; set; }
    public DateTime CollectedAt { get; set; }
    public DateTime ReceivedAt { get; set; }
}
