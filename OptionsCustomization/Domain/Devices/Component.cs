namespace Domain.Devices;

public class Component
{
    public Guid ComponentId { get; set; }
    public long DeviceId { get; set; }
    public ComponentType ComponentType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Capability { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string StateValue { get; set; } = string.Empty;
    public ComponentState State { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
