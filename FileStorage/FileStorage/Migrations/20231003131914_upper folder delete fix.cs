using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorage.Migrations
{
    /// <inheritdoc />
    public partial class upperfolderdeletefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Folders",
                table: "files");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders",
                table: "folders");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Folders",
                table: "files",
                column: "folder_id",
                principalTable: "folders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders",
                table: "folders",
                column: "upper_folder_id",
                principalTable: "folders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Folders",
                table: "files");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders",
                table: "folders");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Folders",
                table: "files",
                column: "folder_id",
                principalTable: "folders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders",
                table: "folders",
                column: "upper_folder_id",
                principalTable: "folders",
                principalColumn: "id");
        }
    }
}
