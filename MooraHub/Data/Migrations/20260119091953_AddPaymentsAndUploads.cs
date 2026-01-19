using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MooraHub.Data.Migrations
{
    public partial class AddPaymentsAndUploads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ Admin voice note
            migrationBuilder.AddColumn<string>(
                name: "AdminVoiceNotePath",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: true);

            // ✅ User unread badge tracking
            migrationBuilder.AddColumn<bool>(
                name: "IsReadByUser",
                table: "SupportTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadByUserAt",
                table: "SupportTickets",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminVoiceNotePath",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "IsReadByUser",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "ReadByUserAt",
                table: "SupportTickets");
        }
    }
}
