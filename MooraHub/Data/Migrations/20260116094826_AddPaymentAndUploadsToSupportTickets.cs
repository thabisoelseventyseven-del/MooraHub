using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MooraHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAndUploadsToSupportTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProofOfPaymentPath",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoiceNotePath",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "ProofOfPaymentPath",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "VoiceNotePath",
                table: "SupportTickets");
        }
    }
}
