using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HC_WEB_FINALPROJECT.Migrations
{
    public partial class pagingmodelagi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "AttendancesPagings");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "AttendancesPagings");

            migrationBuilder.AddColumn<int>(
                name: "CurentPage",
                table: "AttendancesPagings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Search",
                table: "AttendancesPagings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShowItem",
                table: "AttendancesPagings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusPage",
                table: "AttendancesPagings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurentPage",
                table: "AttendancesPagings");

            migrationBuilder.DropColumn(
                name: "Search",
                table: "AttendancesPagings");

            migrationBuilder.DropColumn(
                name: "ShowItem",
                table: "AttendancesPagings");

            migrationBuilder.DropColumn(
                name: "StatusPage",
                table: "AttendancesPagings");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "AttendancesPagings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "AttendancesPagings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
