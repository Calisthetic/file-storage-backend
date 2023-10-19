using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileStorage.Migrations
{
    /// <inheritdoc />
    public partial class usersverify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.AddColumn<bool>(
                name: "is_verify",
                table: "users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "verify_code",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "verify_code",
                table: "emails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_verify",
                table: "users");

            migrationBuilder.DropColumn(
                name: "verify_code",
                table: "users");

            migrationBuilder.DropColumn(
                name: "verify_code",
                table: "emails");

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    answer = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Questions_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_questions_user_id",
                table: "questions",
                column: "user_id");
        }
    }
}
