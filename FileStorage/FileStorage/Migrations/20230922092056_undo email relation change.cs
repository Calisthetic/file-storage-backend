using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorage.Migrations
{
    /// <inheritdoc />
    public partial class undoemailrelationchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Emails",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "primary_email_id",
                table: "users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Emails",
                table: "users",
                column: "primary_email_id",
                principalTable: "emails",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Emails",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "primary_email_id",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Emails",
                table: "users",
                column: "primary_email_id",
                principalTable: "emails",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
