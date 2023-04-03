using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Queue_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class nocap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customers_servicePoints_ServicePointId",
                table: "customers");

            migrationBuilder.DropTable(
                name: "TicketViewModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_servicePoints",
                table: "servicePoints");

            migrationBuilder.RenameTable(
                name: "servicePoints",
                newName: "servicepoints");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "servicepoints",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "servicepoints",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "servicepoints",
                newName: "datecreated");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "servicepoints",
                newName: "createdby");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "servicepoints",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TimeOut",
                table: "customers",
                newName: "timeout");

            migrationBuilder.RenameColumn(
                name: "TimeIn",
                table: "customers",
                newName: "timein");

            migrationBuilder.RenameColumn(
                name: "TicketNumber",
                table: "customers",
                newName: "ticketnumber");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "customers",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ServicePointId",
                table: "customers",
                newName: "servicepointid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "customers",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_customers_ServicePointId",
                table: "customers",
                newName: "IX_customers_servicepointid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_servicepoints",
                table: "servicepoints",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_customers_servicepoints_servicepointid",
                table: "customers",
                column: "servicepointid",
                principalTable: "servicepoints",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customers_servicepoints_servicepointid",
                table: "customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_servicepoints",
                table: "servicepoints");

            migrationBuilder.RenameTable(
                name: "servicepoints",
                newName: "servicePoints");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "servicePoints",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "servicePoints",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "datecreated",
                table: "servicePoints",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "createdby",
                table: "servicePoints",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "servicePoints",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "timeout",
                table: "customers",
                newName: "TimeOut");

            migrationBuilder.RenameColumn(
                name: "timein",
                table: "customers",
                newName: "TimeIn");

            migrationBuilder.RenameColumn(
                name: "ticketnumber",
                table: "customers",
                newName: "TicketNumber");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "customers",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "servicepointid",
                table: "customers",
                newName: "ServicePointId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "customers",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_customers_servicepointid",
                table: "customers",
                newName: "IX_customers_ServicePointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_servicePoints",
                table: "servicePoints",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TicketViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomersInQueue = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TicketNumber = table.Column<string>(type: "text", nullable: false),
                    servicePoint = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketViewModel", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_customers_servicePoints_ServicePointId",
                table: "customers",
                column: "ServicePointId",
                principalTable: "servicePoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
