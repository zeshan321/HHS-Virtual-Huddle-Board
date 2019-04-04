using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HHSBoard.Data.Migrations
{
    public partial class ImpIdeasImplemented : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImpIdeasImplemented",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BoardID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DateComplete = table.Column<DateTime>(nullable: false),
                    DateEnterIntoDatabase = table.Column<string>(nullable: true),
                    EightWs = table.Column<string>(nullable: true),
                    IsPtFamilyInvovlmentOpportunity = table.Column<bool>(nullable: false),
                    JustDoIt = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    PickChart = table.Column<int>(nullable: true),
                    Pillar = table.Column<string>(nullable: true),
                    Problem = table.Column<string>(nullable: true),
                    ProcessObservationCreated = table.Column<bool>(nullable: false),
                    Solution = table.Column<string>(nullable: true),
                    WorkCreated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpIdeasImplemented", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ImpIdeasImplemented_Boards_BoardID",
                        column: x => x.BoardID,
                        principalTable: "Boards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImpIdeasImplemented_BoardID",
                table: "ImpIdeasImplemented",
                column: "BoardID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpIdeasImplemented");
        }
    }
}
