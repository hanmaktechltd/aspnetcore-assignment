using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryKeyCustomerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customers",
                newName: "TicketNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TicketNumber",
                table: "Customers",
                newName: "Id");
        }
    }
}
