using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TicketNumber",
                table: "QueueItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "QueueItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoShow",
                table: "QueueItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueItems_ServicePoints_ServicePointId",
                table: "QueueItems");

            migrationBuilder.DropIndex(
                name: "IX_QueueItems_ServicePointId",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "Finished",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "NoShow",
                table: "QueueItems");

            migrationBuilder.AlterColumn<int>(
                name: "TicketNumber",
                table: "QueueItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(String),
                oldType: "integer");
        }
    }
}
