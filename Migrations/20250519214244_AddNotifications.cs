using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftGenieAPIWebApp.Migrations
{
    public partial class AddNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
    name: "Notifications",
    columns: table => new
    {
        NotificationId = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        UserId = table.Column<int>(type: "int", nullable: false),
        SenderUserId = table.Column<int>(type: "int", nullable: false),
        Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Notifications", x => x.NotificationId);

        table.ForeignKey(
            name: "FK_Notifications_Users_UserId",
            column: x => x.UserId,
            principalTable: "Users",
            principalColumn: "UserId",
            onDelete: ReferentialAction.Cascade);

        table.ForeignKey(
            name: "FK_Notifications_Users_SenderUserId",
            column: x => x.SenderUserId,
            principalTable: "Users",
            principalColumn: "UserId",
            onDelete: ReferentialAction.Restrict);
    });


            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderUserId",
                table: "Notifications",
                column: "SenderUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
