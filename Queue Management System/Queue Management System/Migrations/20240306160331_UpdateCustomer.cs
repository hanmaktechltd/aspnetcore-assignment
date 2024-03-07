using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ServicePoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ServicePoints");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "customers");
        }
    }
}
