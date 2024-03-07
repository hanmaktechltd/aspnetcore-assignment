using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePointForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_waitingModels_ServicePoint",
                table: "waitingModels",
                column: "ServicePoint");

            migrationBuilder.AddForeignKey(
                name: "FK_waitingModels_ServicePoints_ServicePoint",
                table: "waitingModels",
                column: "ServicePoint",
                principalTable: "ServicePoints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_waitingModels_ServicePoints_ServicePoint",
                table: "waitingModels");

            migrationBuilder.DropIndex(
                name: "IX_waitingModels_ServicePoint",
                table: "waitingModels");
        }
    }
}
