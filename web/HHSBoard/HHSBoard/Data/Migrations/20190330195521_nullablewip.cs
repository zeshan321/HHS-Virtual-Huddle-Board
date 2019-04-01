using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace HHSBoard.Data.Migrations
{
    public partial class nullablewip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PickChart",
                table: "WIPs",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAssigned",
                table: "WIPs",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PickChart",
                table: "WIPs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAssigned",
                table: "WIPs",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}