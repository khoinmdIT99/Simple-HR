using Microsoft.EntityFrameworkCore.Migrations;

namespace HC_WEB_FINALPROJECT.Migrations
{
    public partial class tambahleavemodel_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeDepartment",
                table: "LeaveRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "LeaveRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeOccupation",
                table: "LeaveRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeDepartment",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "EmployeeOccupation",
                table: "LeaveRequests");
        }
    }
}
