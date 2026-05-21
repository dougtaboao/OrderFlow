using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAuditReadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderAuditReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAuditReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderAuditReadModels_CorrelationId",
                table: "OrderAuditReadModels",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAuditReadModels_EventType",
                table: "OrderAuditReadModels",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAuditReadModels_OrderId",
                table: "OrderAuditReadModels",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderAuditReadModels");
        }
    }
}
