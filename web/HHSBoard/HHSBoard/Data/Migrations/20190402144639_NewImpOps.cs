using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HHSBoard.Data.Migrations
{
    public partial class NewImpOps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewImpOps",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BoardID = table.Column<int>(nullable: false),
                    DateIdentified = table.Column<DateTime>(nullable: false),
                    EightWs = table.Column<string>(nullable: true),
                    IsPtFamilyInvovlmentOpportunity = table.Column<bool>(nullable: false),
                    JustDoIt = table.Column<string>(nullable: true),
                    Legend = table.Column<string>(nullable: true),
                    PersonIdentifyingOpportunity = table.Column<string>(nullable: true),
                    PickChart = table.Column<int>(nullable: true),
                    Problem = table.Column<string>(nullable: true),
                    StaffWorkingOnOpportunity = table.Column<string>(nullable: true),
                    StrategicGoals = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewImpOps", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NewImpOps_Boards_BoardID",
                        column: x => x.BoardID,
                        principalTable: "Boards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewImpOps_BoardID",
                table: "NewImpOps",
                column: "BoardID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewImpOps");
        }
    }
}
