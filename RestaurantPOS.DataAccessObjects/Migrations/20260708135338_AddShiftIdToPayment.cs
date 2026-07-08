using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantPOS.DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftIdToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShiftId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ShiftId",
                table: "Payments",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Shifts_ShiftId",
                table: "Payments",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Shifts_ShiftId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ShiftId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "Payments");
        }
    }
}
