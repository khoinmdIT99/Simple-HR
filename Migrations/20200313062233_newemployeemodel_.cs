using Microsoft.EntityFrameworkCore.Migrations;

namespace HC_WEB_FINALPROJECT.Migrations
{
    public partial class newemployeemodel_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyContact",
                table: "Employee");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact1",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact2",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact3",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone1",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone2",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone3",
                table: "Employee",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyContact1",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmergencyContact2",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmergencyContact3",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Phone1",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Phone2",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Phone3",
                table: "Employee");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
