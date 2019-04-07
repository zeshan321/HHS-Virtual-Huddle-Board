using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HHSBoard.Data.Migrations
{
    public partial class ChangeRequestUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TableName",
                table: "ChangeRequests",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "AssociatedName",
                table: "ChangeRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssociatedName",
                table: "ChangeRequests");

            migrationBuilder.AlterColumn<string>(
                name: "TableName",
                table: "ChangeRequests",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
