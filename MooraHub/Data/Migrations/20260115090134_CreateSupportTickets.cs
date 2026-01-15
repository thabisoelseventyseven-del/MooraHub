using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MooraHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateSupportTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReplied",
                table: "SupportTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReplied",
                table: "SupportTickets");
        }
    }
}
