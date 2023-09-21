using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileStorage.Migrations
{
    /// <inheritdoc />
    public partial class renaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "access_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    can_download = table.Column<bool>(type: "boolean", nullable: false),
                    can_edit = table.Column<bool>(type: "boolean", nullable: false),
                    require_auth = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_access_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "action_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_action_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message = table.Column<string>(type: "text", nullable: false),
                    message_template = table.Column<string>(type: "text", nullable: true),
                    level = table.Column<string>(type: "text", nullable: true),
                    time_stamp = table.Column<DateTime>(type: "timestamp", nullable: true),
                    exception = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tariffs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    size = table.Column<int>(type: "integer", nullable: false),
                    upload_limit = table.Column<bool>(type: "boolean", nullable: false),
                    customizable = table.Column<bool>(type: "boolean", nullable: false),
                    show_ad = table.Column<bool>(type: "boolean", nullable: false),
                    integration_help = table.Column<bool>(type: "boolean", nullable: false),
                    price = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tariffs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "folder_links",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    folder_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character(20)", fixedLength: true, maxLength: 20, nullable: false),
                    access_type_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_folder_links", x => x.id);
                    table.ForeignKey(
                        name: "FK_FolderLinks_AccessTypes",
                        column: x => x.access_type_id,
                        principalTable: "access_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    action_type_id = table.Column<int>(type: "integer", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_actions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Actions_ActionTypes",
                        column: x => x.action_type_id,
                        principalTable: "action_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "downloads_of_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_downloads_of_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "downloads_of_folders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    folder_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_downloads_of_folders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "elected_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_elected_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "elected_folders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    folder_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_elected_folders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "emails",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    is_verify = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_emails", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    first_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    second_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    about = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    invited_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    primary_email_id = table.Column<int>(type: "integer", nullable: true),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_Emails",
                        column: x => x.primary_email_id,
                        principalTable: "emails",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "folders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    upper_folder_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    access_type_id = table.Column<int>(type: "integer", nullable: true),
                    color = table.Column<string>(type: "character(6)", fixedLength: true, maxLength: 6, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_folders", x => x.id);
                    table.ForeignKey(
                        name: "FK_Folders_Folders",
                        column: x => x.upper_folder_id,
                        principalTable: "folders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Folders_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "shared_folders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    folder_link_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shared_folders", x => x.id);
                    table.ForeignKey(
                        name: "FK_SharedFolders_FolderLinks",
                        column: x => x.folder_link_id,
                        principalTable: "folder_links",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedFolders_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tariffs_of_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tariff_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tariffs_of_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_TariffsOfUsers_Tariffs",
                        column: x => x.tariff_id,
                        principalTable: "tariffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TariffsOfUsers_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    folder_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_Files_Folders",
                        column: x => x.folder_id,
                        principalTable: "folders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Files_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "views_of_folders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    folder_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_views_of_folders", x => x.id);
                    table.ForeignKey(
                        name: "FK_ViewsOfFolders_Folders",
                        column: x => x.folder_id,
                        principalTable: "folders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewsOfFolders_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "views_of_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_views_of_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_ViewsOfFiles_Files",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewsOfFiles_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_actions_action_type_id",
                table: "actions",
                column: "action_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_actions_user_id",
                table: "actions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_downloads_of_files_file_id",
                table: "downloads_of_files",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_downloads_of_files_user_id",
                table: "downloads_of_files",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_downloads_of_folders_folder_id",
                table: "downloads_of_folders",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "ix_downloads_of_folders_user_id",
                table: "downloads_of_folders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_elected_files_file_id",
                table: "elected_files",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_elected_files_user_id",
                table: "elected_files",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_elected_folders_folder_id",
                table: "elected_folders",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "ix_elected_folders_user_id",
                table: "elected_folders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_emails_user_id",
                table: "emails",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_folder_id",
                table: "files",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_user_id",
                table: "files",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_folder_links_access_type_id",
                table: "folder_links",
                column: "access_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_folders_upper_folder_id",
                table: "folders",
                column: "upper_folder_id");

            migrationBuilder.CreateIndex(
                name: "ix_folders_user_id",
                table: "folders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_shared_folders_folder_link_id",
                table: "shared_folders",
                column: "folder_link_id");

            migrationBuilder.CreateIndex(
                name: "ix_shared_folders_user_id",
                table: "shared_folders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tariffs_of_users_tariff_id",
                table: "tariffs_of_users",
                column: "tariff_id");

            migrationBuilder.CreateIndex(
                name: "ix_tariffs_of_users_user_id",
                table: "tariffs_of_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_primary_email_id",
                table: "users",
                column: "primary_email_id");

            migrationBuilder.CreateIndex(
                name: "ix_views_of_files_file_id",
                table: "views_of_files",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_views_of_files_user_id",
                table: "views_of_files",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_views_of_folders_folder_id",
                table: "views_of_folders",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "ix_views_of_folders_user_id",
                table: "views_of_folders",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Users",
                table: "actions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadsOfFiles_Files",
                table: "downloads_of_files",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadsOfFiles_Users",
                table: "downloads_of_files",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadsOfFolders_Folders",
                table: "downloads_of_folders",
                column: "folder_id",
                principalTable: "folders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadsOfFolders_Users",
                table: "downloads_of_folders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectedFiles_Files",
                table: "elected_files",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectedFiles_Users",
                table: "elected_files",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectedFolders_Folders",
                table: "elected_folders",
                column: "folder_id",
                principalTable: "folders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectedFolders_Users",
                table: "elected_folders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Users",
                table: "emails",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Users",
                table: "emails");

            migrationBuilder.DropTable(
                name: "actions");

            migrationBuilder.DropTable(
                name: "downloads_of_files");

            migrationBuilder.DropTable(
                name: "downloads_of_folders");

            migrationBuilder.DropTable(
                name: "elected_files");

            migrationBuilder.DropTable(
                name: "elected_folders");

            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "shared_folders");

            migrationBuilder.DropTable(
                name: "tariffs_of_users");

            migrationBuilder.DropTable(
                name: "views_of_files");

            migrationBuilder.DropTable(
                name: "views_of_folders");

            migrationBuilder.DropTable(
                name: "action_types");

            migrationBuilder.DropTable(
                name: "folder_links");

            migrationBuilder.DropTable(
                name: "tariffs");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "access_types");

            migrationBuilder.DropTable(
                name: "folders");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "emails");
        }
    }
}
