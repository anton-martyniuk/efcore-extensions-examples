using Domain.Devices;

namespace Api.Features.Devices.Models;

public record ComponentRequest(
    long DeviceId,
    ComponentType ComponentType,
    string Name,
    string Capability,
    string Unit,
    string StateValue,
    ComponentState State,
    bool IsActive,
    DateTime LastUpdatedAt
);
