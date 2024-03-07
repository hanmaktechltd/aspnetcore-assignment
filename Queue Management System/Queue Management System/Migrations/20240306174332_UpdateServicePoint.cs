using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServicePoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "ServicePoints",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoShow",
                table: "ServicePoints",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finished",
                table: "ServicePoints");

            migrationBuilder.DropColumn(
                name: "NoShow",
                table: "ServicePoints");
        }
    }
}
