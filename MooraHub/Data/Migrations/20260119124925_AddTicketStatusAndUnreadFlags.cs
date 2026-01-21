
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MooraHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketStatusAndUnreadFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "UserUnreadReply",
                table: "SupportTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "UserUnreadReply",
                table: "SupportTickets");
        }
    }
}
