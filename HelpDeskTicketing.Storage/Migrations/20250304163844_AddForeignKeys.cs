using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskTicketing.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AssignedTo",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TicketId1",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TicketId1",
                table: "Comments",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tickets_TicketId1",
                table: "Comments",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TicketId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "AssignedTo",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
