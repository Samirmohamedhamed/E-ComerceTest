using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EComerceTestSamir.Migrations
{
    /// <inheritdoc />
    public partial class AddTable_OTP_Number : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserOTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OTP = table.Column<int>(type: "int", nullable: false),
                    ApplicationUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    applicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserOTPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserOTPs_AspNetUsers_applicationUserId",
                        column: x => x.applicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOTPs_applicationUserId",
                table: "ApplicationUserOTPs",
                column: "applicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserOTPs");
        }
    }
}
