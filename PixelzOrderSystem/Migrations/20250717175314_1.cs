using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelzOrderSystem.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderProcessingSagas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStep = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProcessingSagas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SagaStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OrderProcessingSagaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SagaStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SagaStep_OrderProcessingSagas_OrderProcessingSagaId",
                        column: x => x.OrderProcessingSagaId,
                        principalTable: "OrderProcessingSagas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SagaStep_OrderProcessingSagaId",
                table: "SagaStep",
                column: "OrderProcessingSagaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SagaStep");

            migrationBuilder.DropTable(
                name: "OrderProcessingSagas");
        }
    }
}
