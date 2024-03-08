using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQueue1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueItems_ServicePoints_ServicePointId",
                table: "QueueItems");

            migrationBuilder.DropIndex(
                name: "IX_QueueItems_ServicePointId",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "ServicePointId",
                table: "QueueItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicePointId",
                table: "QueueItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_ServicePointId",
                table: "QueueItems",
                column: "ServicePointId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueItems_ServicePoints_ServicePointId",
                table: "QueueItems",
                column: "ServicePointId",
                principalTable: "ServicePoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
