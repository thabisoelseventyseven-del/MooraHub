using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MooraHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsAndUploads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminVoiceNotePath",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminVoiceNotePath",
                table: "SupportTickets");
        }
    }
}
