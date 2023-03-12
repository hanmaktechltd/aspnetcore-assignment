using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class customers1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
