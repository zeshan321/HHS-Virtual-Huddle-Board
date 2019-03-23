using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HHSBoard.Data.Migrations
{
    public partial class Celebrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Celebrations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BoardID = table.Column<int>(nullable: false),
                    What = table.Column<string>(nullable: true),
                    Who = table.Column<string>(nullable: true),
                    Why = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Celebrations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Celebrations_Boards_BoardID",
                        column: x => x.BoardID,
                        principalTable: "Boards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Celebrations_BoardID",
                table: "Celebrations",
                column: "BoardID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Celebrations");
        }
    }
}
