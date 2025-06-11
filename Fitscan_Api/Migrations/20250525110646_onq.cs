using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitscan_Api.Migrations
{
    /// <inheritdoc />
    public partial class onq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gymcode",
                table: "PaymentDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gymcode",
                table: "PaymentDetails");
        }
    }
}
