using Domain.Devices;

namespace Api.Features.Devices.Models;

public record TelemetryRequest(
    long DeviceId,
    Guid ComponentId,
    double Value,
    TelemetryQuality Quality,
    DateTime CollectedAt,
    DateTime ReceivedAt
);
