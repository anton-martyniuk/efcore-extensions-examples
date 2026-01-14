using Domain.Devices;

namespace Api.Features.Devices.Models;

public record DeviceRequest(
    string Name,
    string DeviceType,
    string Manufacturer,
    string SerialNumber,
    string FirmwareVersion,
    string HardwareVersion,
    DeviceStatus Status,
    DateTime LastSeenAt,
    DateTime RegisteredAt,
    string Configuration
);
