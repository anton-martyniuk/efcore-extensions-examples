using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "iot_devices");

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "iot_devices",
                columns: table => new
                {
                    device_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    device_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    manufacturer = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    serial_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    firmware_version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    hardware_version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    last_seen_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    registered_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    configuration = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.device_id);
                });

            migrationBuilder.CreateTable(
                name: "components",
                schema: "iot_devices",
                columns: table => new
                {
                    component_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    device_id = table.Column<long>(type: "bigint", nullable: false),
                    component_type = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    capability = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    state_value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    state = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_components", x => x.component_id);
                    table.ForeignKey(
                        name: "fk_components_devices_device_id",
                        column: x => x.device_id,
                        principalSchema: "iot_devices",
                        principalTable: "devices",
                        principalColumn: "device_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telemetries",
                schema: "iot_devices",
                columns: table => new
                {
                    telemetry_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    device_id = table.Column<long>(type: "bigint", nullable: false),
                    component_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    value = table.Column<double>(type: "float", nullable: false),
                    quality = table.Column<int>(type: "int", nullable: false),
                    collected_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    received_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telemetries", x => x.telemetry_id);
                    table.ForeignKey(
                        name: "fk_telemetries_components_component_id",
                        column: x => x.component_id,
                        principalSchema: "iot_devices",
                        principalTable: "components",
                        principalColumn: "component_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_telemetries_devices_device_id",
                        column: x => x.device_id,
                        principalSchema: "iot_devices",
                        principalTable: "devices",
                        principalColumn: "device_id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_components_device_id",
                schema: "iot_devices",
                table: "components",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_telemetries_component_id",
                schema: "iot_devices",
                table: "telemetries",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "ix_telemetries_device_id",
                schema: "iot_devices",
                table: "telemetries",
                column: "device_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telemetries",
                schema: "iot_devices");

            migrationBuilder.DropTable(
                name: "components",
                schema: "iot_devices");

            migrationBuilder.DropTable(
                name: "devices",
                schema: "iot_devices");
        }
    }
}
