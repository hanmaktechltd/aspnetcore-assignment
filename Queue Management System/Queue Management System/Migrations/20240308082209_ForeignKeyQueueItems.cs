using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyQueueItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_ServicePoint",
                table: "QueueItems",
                column: "ServicePoint");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueItems_ServicePoints_ServicePoint",
                table: "QueueItems",
                column: "ServicePoint",
                principalTable: "ServicePoints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueItems_ServicePoints_ServicePoint",
                table: "QueueItems");

            migrationBuilder.DropIndex(
                name: "IX_QueueItems_ServicePoint",
                table: "QueueItems");
        }
    }
}
