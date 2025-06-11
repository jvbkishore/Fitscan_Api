using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fitscan_Api.Migrations
{
    /// <inheritdoc />
    public partial class oone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "PaymentDetails",
                newName: "UpdatedOn");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PaymentDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ConsistencyScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    CurrentStreak = table.Column<int>(type: "integer", nullable: false),
                    LongestStreak = table.Column<int>(type: "integer", nullable: false),
                    WeeklyAverage = table.Column<double>(type: "double precision", nullable: false),
                    MonthlyVisits = table.Column<int>(type: "integer", nullable: false),
                    LastVisitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImprovementPercent = table.Column<int>(type: "integer", nullable: false),
                    ReferralsThisMonth = table.Column<int>(type: "integer", nullable: false),
                    TriedNewClass = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedChallenge = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsistencyScores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferralInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferralCode = table.Column<string>(type: "text", nullable: false),
                    ReferralLink = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    GymId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferralStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendName = table.Column<string>(type: "text", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reward = table.Column<string>(type: "text", nullable: false),
                    ReferralInfoId = table.Column<int>(type: "integer", nullable: false),
                    UsedByUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralStatus_ReferralInfo_ReferralInfoId",
                        column: x => x.ReferralInfoId,
                        principalTable: "ReferralInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferralStatus_ReferralInfoId",
                table: "ReferralStatus",
                column: "ReferralInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsistencyScores");

            migrationBuilder.DropTable(
                name: "ReferralStatus");

            migrationBuilder.DropTable(
                name: "ReferralInfo");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PaymentDetails");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "PaymentDetails",
                newName: "UpdatedAt");
        }
    }
}
