﻿// <auto-generated />
using System;
using FileStorage.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileStorage.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20230924134900_add token")]
    partial class addtoken
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FileStorage.Models.Db.AccessType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("CanDownload")
                        .HasColumnType("boolean")
                        .HasColumnName("can_download");

                    b.Property<bool>("CanEdit")
                        .HasColumnType("boolean")
                        .HasColumnName("can_edit");

                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("name");

                    b.Property<bool>("RequireAuth")
                        .HasColumnType("boolean")
                        .HasColumnName("require_auth");

                    b.HasKey("Id")
                        .HasName("pk_access_types");

                    b.ToTable("access_types", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.Action", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ActionTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("action_type_id");

                    b.Property<int>("Count")
                        .HasColumnType("integer")
                        .HasColumnName("count");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("created_at");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_actions");

                    b.HasIndex("ActionTypeId")
                        .HasDatabaseName("ix_actions_action_type_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_actions_user_id");

                    b.ToTable("actions", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.ActionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_action_types");

                    b.ToTable("action_types", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.DownloadOfFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FileId")
                        .HasColumnType("integer")
                        .HasColumnName("file_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_downloads_of_files");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_downloads_of_files_file_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_downloads_of_files_user_id");

                    b.ToTable("downloads_of_files", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.DownloadOfFolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FolderId")
                        .HasColumnType("integer")
                        .HasColumnName("folder_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_downloads_of_folders");

                    b.HasIndex("FolderId")
                        .HasDatabaseName("ix_downloads_of_folders_folder_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_downloads_of_folders_user_id");

                    b.ToTable("downloads_of_folders", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.ElectedFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FileId")
                        .HasColumnType("integer")
                        .HasColumnName("file_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_elected_files");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_elected_files_file_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_elected_files_user_id");

                    b.ToTable("elected_files", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.ElectedFolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FolderId")
                        .HasColumnType("integer")
                        .HasColumnName("folder_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_elected_folders");

                    b.HasIndex("FolderId")
                        .HasDatabaseName("ix_elected_folders_folder_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_elected_folders_user_id");

                    b.ToTable("elected_folders", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVerify")
                        .HasColumnType("boolean")
                        .HasColumnName("is_verify");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_emails");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_emails_user_id");

                    b.ToTable("emails", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.File", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("created_at");

                    b.Property<int>("FileTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("file_type_id");

                    b.Property<int?>("FolderId")
                        .HasColumnType("integer")
                        .HasColumnName("folder_id");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("name");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_files");

                    b.HasIndex("FileTypeId")
                        .HasDatabaseName("ix_files_file_type_id");

                    b.HasIndex("FolderId")
                        .HasDatabaseName("ix_files_folder_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_files_user_id");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.FileType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_file_types");

                    b.ToTable("file_types", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.Folder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccessTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("access_type_id");

                    b.Property<string>("Color")
                        .HasMaxLength(6)
                        .HasColumnType("character(6)")
                        .HasColumnName("color")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("name");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character(32)")
                        .HasColumnName("token")
                        .IsFixedLength();

                    b.Property<int?>("UpperFolderId")
                        .HasColumnType("integer")
                        .HasColumnName("upper_folder_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_folders");

                    b.HasIndex("AccessTypeId")
                        .HasDatabaseName("ix_folders_access_type_id");

                    b.HasIndex("UpperFolderId")
                        .HasDatabaseName("ix_folders_upper_folder_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_folders_user_id");

                    b.ToTable("folders", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.FolderLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("access_type_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("EndAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("end_at");

                    b.Property<int>("FolderId")
                        .HasColumnType("integer")
                        .HasColumnName("folder_id");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character(40)")
                        .HasColumnName("token")
                        .IsFixedLength();

                    b.HasKey("Id")
                        .HasName("pk_folder_links");

                    b.HasIndex("AccessTypeId")
                        .HasDatabaseName("ix_folder_links_access_type_id");

                    b.ToTable("folder_links", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Exception")
                        .HasColumnType("text")
                        .HasColumnName("exception");

                    b.Property<string>("Level")
                        .HasColumnType("text")
                        .HasColumnName("level");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<string>("MessageTemplate")
                        .HasColumnType("text")
                        .HasColumnName("message_template");

                    b.Property<string>("Properties")
                        .HasColumnType("text")
                        .HasColumnName("properties");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("timestamp")
                        .HasColumnName("time_stamp");

                    b.HasKey("Id")
                        .HasName("pk_logs");

                    b.ToTable("logs", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.SharedFolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FolderLinkId")
                        .HasColumnType("integer")
                        .HasColumnName("folder_link_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_shared_folders");

                    b.HasIndex("FolderLinkId")
                        .HasDatabaseName("ix_shared_folders_folder_link_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_shared_folders_user_id");

                    b.ToTable("shared_folders", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.Tariff", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Customizable")
                        .HasColumnType("boolean")
                        .HasColumnName("customizable");

                    b.Property<bool>("IntegrationHelp")
                        .HasColumnType("boolean")
                        .HasColumnName("integration_help");

                    b.Property<decimal>("Price")
                        .HasColumnType("money")
                        .HasColumnName("price");

                    b.Property<bool>("ShowAd")
                        .HasColumnType("boolean")
                        .HasColumnName("show_ad");

                    b.Property<int>("Size")
                        .HasColumnType("integer")
                        .HasColumnName("size");

                    b.Property<bool>("UploadLimit")
                        .HasColumnType("boolean")
                        .HasColumnName("upload_limit");

                    b.HasKey("Id")
                        .HasName("pk_tariffs");

                    b.ToTable("tariffs", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.TariffOfUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("end_at");

                    b.Property<int>("TariffId")
                        .HasColumnType("integer")
                        .HasColumnName("tariff_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_tariffs_of_users");

                    b.HasIndex("TariffId")
                        .HasDatabaseName("ix_tariffs_of_users_tariff_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_tariffs_of_users_user_id");

                    b.ToTable("tariffs_of_users", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("About")
                        .HasColumnType("text")
                        .HasColumnName("about");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("timestamp")
                        .HasColumnName("birthday");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("created_at");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("first_name");

                    b.Property<int?>("InvitedBy")
                        .HasColumnType("integer")
                        .HasColumnName("invited_by");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_blocked");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("password");

                    b.Property<int?>("PrimaryEmailId")
                        .HasColumnType("integer")
                        .HasColumnName("primary_email_id");

                    b.Property<string>("SecondName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("second_name");

                    b.Property<string>("Username")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("PrimaryEmailId")
                        .HasDatabaseName("ix_users_primary_email_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.ViewOfFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FileId")
                        .HasColumnType("integer")
                        .HasColumnName("file_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_views_of_files");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_views_of_files_file_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_views_of_files_user_id");

                    b.ToTable("views_of_files", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.ViewOfFolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FolderId")
                        .HasColumnType("integer")
                        .HasColumnName("folder_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_views_of_folders");

                    b.HasIndex("FolderId")
                        .HasDatabaseName("ix_views_of_folders_folder_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_views_of_folders_user_id");

                    b.ToTable("views_of_folders", (string)null);
                });

            modelBuilder.Entity("FileStorage.Models.Db.Action", b =>
                {
                    b.HasOne("FileStorage.Models.Db.ActionType", "ActionType")
                        .WithMany("Actions")
                        .HasForeignKey("ActionTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_Actions_ActionTypes");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("Actions")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Actions_Users");

                    b.Navigation("ActionType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.DownloadOfFile", b =>
                {
                    b.HasOne("FileStorage.Models.Db.File", "File")
                        .WithMany("DownloadsOfFiles")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DownloadsOfFiles_Files");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("DownloadsOfFiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DownloadsOfFiles_Users");

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.DownloadOfFolder", b =>
                {
                    b.HasOne("FileStorage.Models.Db.Folder", "Folder")
                        .WithMany("DownloadsOfFolders")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DownloadsOfFolders_Folders");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("DownloadsOfFolders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DownloadsOfFolders_Users");

                    b.Navigation("Folder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.ElectedFile", b =>
                {
                    b.HasOne("FileStorage.Models.Db.File", "File")
                        .WithMany("ElectedFiles")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ElectedFiles_Files");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("ElectedFiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ElectedFiles_Users");

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.ElectedFolder", b =>
                {
                    b.HasOne("FileStorage.Models.Db.Folder", "Folder")
                        .WithMany("ElectedFolders")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ElectedFolders_Folders");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("ElectedFolders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ElectedFolders_Users");

                    b.Navigation("Folder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.Email", b =>
                {
                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("Emails")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Emails_Users");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.File", b =>
                {
                    b.HasOne("FileStorage.Models.Db.FileType", "FileType")
                        .WithMany("Files")
                        .HasForeignKey("FileTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Files_FileTypes");

                    b.HasOne("FileStorage.Models.Db.Folder", "Folder")
                        .WithMany("Files")
                        .HasForeignKey("FolderId")
                        .HasConstraintName("FK_Files_Folders");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("Files")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Files_Users");

                    b.Navigation("FileType");

                    b.Navigation("Folder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.Folder", b =>
                {
                    b.HasOne("FileStorage.Models.Db.AccessType", "AccessType")
                        .WithMany("Folders")
                        .HasForeignKey("AccessTypeId")
                        .HasConstraintName("FK_Folders_AccessTypes");

                    b.HasOne("FileStorage.Models.Db.Folder", "UpperFolder")
                        .WithMany("InverseUpperFolder")
                        .HasForeignKey("UpperFolderId")
                        .HasConstraintName("FK_Folders_Folders");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("Folders")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Folders_Users");

                    b.Navigation("AccessType");

                    b.Navigation("UpperFolder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.FolderLink", b =>
                {
                    b.HasOne("FileStorage.Models.Db.AccessType", "AccessType")
                        .WithMany("FolderLinks")
                        .HasForeignKey("AccessTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_FolderLinks_AccessTypes");

                    b.Navigation("AccessType");
                });

            modelBuilder.Entity("FileStorage.Models.Db.SharedFolder", b =>
                {
                    b.HasOne("FileStorage.Models.Db.FolderLink", "FolderLink")
                        .WithMany()
                        .HasForeignKey("FolderLinkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_SharedFolders_FolderLinks");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_SharedFolders_Users");

                    b.Navigation("FolderLink");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.TariffOfUser", b =>
                {
                    b.HasOne("FileStorage.Models.Db.Tariff", "Tariff")
                        .WithMany("TariffsOfUsers")
                        .HasForeignKey("TariffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TariffsOfUsers_Tariffs");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("TariffsOfUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TariffsOfUsers_Users");

                    b.Navigation("Tariff");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.User", b =>
                {
                    b.HasOne("FileStorage.Models.Db.Email", "PrimaryEmail")
                        .WithMany("Users")
                        .HasForeignKey("PrimaryEmailId")
                        .HasConstraintName("FK_Users_Emails");

                    b.Navigation("PrimaryEmail");
                });

            modelBuilder.Entity("FileStorage.Models.Db.ViewOfFile", b =>
                {
                    b.HasOne("FileStorage.Models.Db.File", "File")
                        .WithMany("ViewsOfFiles")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ViewsOfFiles_Files");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("ViewsOfFiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ViewsOfFiles_Users");

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.ViewOfFolder", b =>
                {
                    b.HasOne("FileStorage.Models.Db.Folder", "Folder")
                        .WithMany("ViewsOfFolders")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ViewsOfFolders_Folders");

                    b.HasOne("FileStorage.Models.Db.User", "User")
                        .WithMany("ViewsOfFolders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ViewsOfFolders_Users");

                    b.Navigation("Folder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileStorage.Models.Db.AccessType", b =>
                {
                    b.Navigation("FolderLinks");

                    b.Navigation("Folders");
                });

            modelBuilder.Entity("FileStorage.Models.Db.ActionType", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("FileStorage.Models.Db.Email", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("FileStorage.Models.Db.File", b =>
                {
                    b.Navigation("DownloadsOfFiles");

                    b.Navigation("ElectedFiles");

                    b.Navigation("ViewsOfFiles");
                });

            modelBuilder.Entity("FileStorage.Models.Db.FileType", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("FileStorage.Models.Db.Folder", b =>
                {
                    b.Navigation("DownloadsOfFolders");

                    b.Navigation("ElectedFolders");

                    b.Navigation("Files");

                    b.Navigation("InverseUpperFolder");

                    b.Navigation("ViewsOfFolders");
                });

            modelBuilder.Entity("FileStorage.Models.Db.Tariff", b =>
                {
                    b.Navigation("TariffsOfUsers");
                });

            modelBuilder.Entity("FileStorage.Models.Db.User", b =>
                {
                    b.Navigation("Actions");

                    b.Navigation("DownloadsOfFiles");

                    b.Navigation("DownloadsOfFolders");

                    b.Navigation("ElectedFiles");

                    b.Navigation("ElectedFolders");

                    b.Navigation("Emails");

                    b.Navigation("Files");

                    b.Navigation("Folders");

                    b.Navigation("TariffsOfUsers");

                    b.Navigation("ViewsOfFiles");

                    b.Navigation("ViewsOfFolders");
                });
#pragma warning restore 612, 618
        }
    }
}
