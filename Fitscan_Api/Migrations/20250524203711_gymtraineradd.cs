using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitscan_Api.Migrations
{
    /// <inheritdoc />
    public partial class gymtraineradd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Userplanexpiry",
                table: "GymUsers",
                newName: "UserPlanJoiningDate");

            migrationBuilder.AddColumn<string>(
                name: "Traineremail",
                table: "GymUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserPlanExpiryDate",
                table: "GymUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Traineremail",
                table: "GymUsers");

            migrationBuilder.DropColumn(
                name: "UserPlanExpiryDate",
                table: "GymUsers");

            migrationBuilder.RenameColumn(
                name: "UserPlanJoiningDate",
                table: "GymUsers",
                newName: "Userplanexpiry");
        }
    }
}
