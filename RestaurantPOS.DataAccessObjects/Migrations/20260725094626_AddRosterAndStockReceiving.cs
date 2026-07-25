using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantPOS.DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddRosterAndStockReceiving : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScheduledEndTime",
                table: "Users",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScheduledStartTime",
                table: "Users",
                type: "time",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IngredientStockEntries",
                columns: table => new
                {
                    IngredientStockEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuantityAdded = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientStockEntries", x => x.IngredientStockEntryId);
                    table.ForeignKey(
                        name: "FK_IngredientStockEntries_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "IngredientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientStockEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "ScheduledEndTime", "ScheduledStartTime" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientStockEntries_IngredientId",
                table: "IngredientStockEntries",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientStockEntries_UserId",
                table: "IngredientStockEntries",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientStockEntries");

            migrationBuilder.DropColumn(
                name: "ScheduledEndTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ScheduledStartTime",
                table: "Users");
        }
    }
}
