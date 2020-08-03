using Microsoft.EntityFrameworkCore.Migrations;

namespace HC_WEB_FINALPROJECT.Migrations
{
    public partial class modelupdatelagi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Attendances");

            migrationBuilder.AddColumn<string>(
                name: "Remarks_in",
                table: "Attendances",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks_out",
                table: "Attendances",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Attendances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks_in",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Remarks_out",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Attendances");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
