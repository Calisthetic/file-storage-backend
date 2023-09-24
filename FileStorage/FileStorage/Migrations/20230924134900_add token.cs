using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorage.Migrations
{
    /// <inheritdoc />
    public partial class addtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "folders",
                type: "character(32)",
                fixedLength: true,
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "folder_links",
                type: "character(40)",
                fixedLength: true,
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldFixedLength: true,
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "ix_folders_access_type_id",
                table: "folders",
                column: "access_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_AccessTypes",
                table: "folders",
                column: "access_type_id",
                principalTable: "access_types",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_AccessTypes",
                table: "folders");

            migrationBuilder.DropIndex(
                name: "ix_folders_access_type_id",
                table: "folders");

            migrationBuilder.DropColumn(
                name: "token",
                table: "folders");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "folder_links",
                type: "character(20)",
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(40)",
                oldFixedLength: true,
                oldMaxLength: 40);
        }
    }
}
