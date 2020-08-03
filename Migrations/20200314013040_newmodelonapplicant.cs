using Microsoft.EntityFrameworkCore.Migrations;

namespace HC_WEB_FINALPROJECT.Migrations
{
    public partial class newmodelonapplicant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyContact",
                table: "Applicant");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact1",
                table: "Applicant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact2",
                table: "Applicant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact3",
                table: "Applicant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone1",
                table: "Applicant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone2",
                table: "Applicant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone3",
                table: "Applicant",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyContact1",
                table: "Applicant");

            migrationBuilder.DropColumn(
                name: "EmergencyContact2",
                table: "Applicant");

            migrationBuilder.DropColumn(
                name: "EmergencyContact3",
                table: "Applicant");

            migrationBuilder.DropColumn(
                name: "Phone1",
                table: "Applicant");

            migrationBuilder.DropColumn(
                name: "Phone2",
                table: "Applicant");

            migrationBuilder.DropColumn(
                name: "Phone3",
                table: "Applicant");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact",
                table: "Applicant",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
