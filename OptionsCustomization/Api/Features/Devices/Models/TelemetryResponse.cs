using Domain.Devices;

namespace Api.Features.Devices.Models;

public record TelemetryResponse(
    Guid TelemetryId,
    long DeviceId,
    Guid ComponentId,
    double Value,
    TelemetryQuality Quality,
    DateTime CollectedAt,
    DateTime ReceivedAt
);
