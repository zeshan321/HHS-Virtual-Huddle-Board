using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HHSBoard.Data.Migrations
{
    public partial class WIP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WIPs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BoardID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DateAssigned = table.Column<DateTime>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    EightWs = table.Column<string>(nullable: true),
                    IsPtFamilyInvovlmentOpportunity = table.Column<bool>(nullable: false),
                    JustDoIt = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PickChart = table.Column<int>(nullable: false),
                    Problem = table.Column<string>(nullable: true),
                    Saftey = table.Column<string>(nullable: true),
                    StaffWorkingOnOpportunity = table.Column<string>(nullable: true),
                    StrategicGoals = table.Column<string>(nullable: true),
                    Updates = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    Why = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WIPs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WIPs_Boards_BoardID",
                        column: x => x.BoardID,
                        principalTable: "Boards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WIPs_BoardID",
                table: "WIPs",
                column: "BoardID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WIPs");
        }
    }
}
