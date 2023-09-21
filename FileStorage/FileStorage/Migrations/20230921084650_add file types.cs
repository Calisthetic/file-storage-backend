using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileStorage.Migrations
{
    /// <inheritdoc />
    public partial class addfiletypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "file_type_id",
                table: "files",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "file_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_types", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_files_file_type_id",
                table: "files",
                column: "file_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileTypes",
                table: "files",
                column: "file_type_id",
                principalTable: "file_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileTypes",
                table: "files");

            migrationBuilder.DropTable(
                name: "file_types");

            migrationBuilder.DropIndex(
                name: "ix_files_file_type_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "file_type_id",
                table: "files");
        }
    }
}
