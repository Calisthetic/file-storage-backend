using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorage.Migrations
{
    /// <inheritdoc />
    public partial class FileTypenamefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "file_types",
                newName: "text");

            migrationBuilder.AlterColumn<string>(
                name: "text",
                table: "file_types",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "text",
                table: "file_types",
                newName: "name");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "file_types",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
