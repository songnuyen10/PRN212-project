using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantPOS.DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddOpenedByUserToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");

            // Default to the seeded admin (UserId 1) for any pre-existing rows from
            // local dev/testing — every one of them was in fact opened by that user.
            migrationBuilder.AddColumn<int>(
                name: "OpenedByUserId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OpenedByUserId",
                table: "Orders",
                column: "OpenedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_OpenedByUserId",
                table: "Orders",
                column: "OpenedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_OpenedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OpenedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OpenedByUserId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
