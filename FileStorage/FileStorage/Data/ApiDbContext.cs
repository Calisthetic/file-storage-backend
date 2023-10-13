using FileStorage.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Drawing.Printing;

namespace FileStorage.Data
{
    public class ApiDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public ApiDbContext(DbContextOptions<ApiDbContext> options, IConfiguration configuration) :base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<AccessType> AccessTypes { get; set; }
        public virtual DbSet<Models.Db.Action> Actions { get; set; }
        public virtual DbSet<ActionType> ActionTypes { get; set; }
        public virtual DbSet<DownloadOfFile> DownloadsOfFiles { get; set; }
        public virtual DbSet<DownloadOfFolder> DownloadsOfFolders { get; set; }
        public virtual DbSet<ElectedFile> ElectedFiles { get; set; }
        public virtual DbSet<ElectedFolder> ElectedFolders { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<Models.Db.File> Files { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<FolderLink> FolderLinks { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<SharedFolder> SharedFolders { get; set; }
        public virtual DbSet<Tariff> Tariffs { get; set; }
        public virtual DbSet<TariffOfUser> TariffsOfUsers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ViewOfFile> ViewsOfFiles { get; set; }
        public virtual DbSet<ViewOfFolder> ViewsOfFolders { get; set; }
        public virtual DbSet<Question> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DatabaseConnection"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccessType>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<Models.Db.Action>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");

                entity.HasOne(d => d.ActionType).WithMany(p => p.Actions)
                    .HasForeignKey(d => d.ActionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Actions_ActionTypes");

                entity.HasOne(d => d.User).WithMany(p => p.Actions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Actions_Users");
            });

            modelBuilder.Entity<ActionType>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<DownloadOfFile>(entity =>
            {
                entity.HasOne(d => d.File).WithMany(p => p.DownloadsOfFiles)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DownloadsOfFiles_Files");

                entity.HasOne(d => d.User).WithMany(p => p.DownloadsOfFiles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_DownloadsOfFiles_Users");
            });

            modelBuilder.Entity<DownloadOfFolder>(entity =>
            {
                entity.HasOne(d => d.Folder).WithMany(p => p.DownloadsOfFolders)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DownloadsOfFolders_Folders");

                entity.HasOne(d => d.User).WithMany(p => p.DownloadsOfFolders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_DownloadsOfFolders_Users");
            });

            modelBuilder.Entity<ElectedFile>(entity =>
            {
                entity.HasOne(d => d.File).WithMany(p => p.ElectedFiles)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ElectedFiles_Files");

                entity.HasOne(d => d.User).WithMany(p => p.ElectedFiles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ElectedFiles_Users");
            });

            modelBuilder.Entity<ElectedFolder>(entity =>
            {
                entity.HasOne(d => d.Folder).WithMany(p => p.ElectedFolders)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ElectedFolders_Folders");

                entity.HasOne(d => d.User).WithMany(p => p.ElectedFolders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ElectedFolders_Users");
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(30);

                entity.HasOne(d => d.User).WithMany(p => p.Emails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Emails_Users");
            });

            modelBuilder.Entity<Models.Db.File>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                entity.Property(e => e.Name).HasColumnType("text");

                entity.HasOne(d => d.Folder).WithMany(p => p.Files)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Files_Folders");

                entity.HasOne(d => d.FileType).WithMany(p => p.Files)
                    .HasForeignKey(d => d.FileTypeId)
                    .HasConstraintName("FK_Files_FileTypes");

                entity.HasOne(d => d.User).WithMany(p => p.Files)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Files_Users");
            });

            modelBuilder.Entity<FileType>(entity =>
            {
                entity.Property(e => e.Name).HasColumnName("text");
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.Color)
                    .HasMaxLength(6)
                    .IsFixedLength();
                entity.Property(e => e.Token)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                entity.Property(e => e.Name).HasMaxLength(20);

                entity.HasOne(d => d.UpperFolder).WithMany(p => p.InverseUpperFolder)
                    .HasForeignKey(d => d.UpperFolderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Folders_Folders");

                entity.HasOne(d => d.User).WithMany(p => p.Folders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Folders_Users");

                entity.HasOne(d => d.AccessType).WithMany(p => p.Folders)
                    .HasForeignKey(d => d.AccessTypeId)
                    .HasConstraintName("FK_Folders_AccessTypes");
            });

            modelBuilder.Entity<FolderLink>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                entity.Property(e => e.EndAt).HasColumnType("timestamp");
                entity.Property(e => e.Token)
                    .HasMaxLength(40)
                    .IsFixedLength();

                entity.HasOne(d => d.AccessType).WithMany(p => p.FolderLinks)
                    .HasForeignKey(d => d.AccessTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FolderLinks_AccessTypes");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Exception).HasColumnType("text");
                entity.Property(e => e.Level).HasColumnType("text");
                entity.Property(e => e.Message).HasColumnType("text");
                entity.Property(e => e.MessageTemplate).HasColumnType("text");
                entity.Property(e => e.Properties).HasColumnType("text");
                entity.Property(e => e.TimeStamp).HasColumnType("timestamp");
            });

            modelBuilder.Entity<SharedFolder>(entity =>
            {
                entity.HasOne(d => d.FolderLink).WithMany()
                    .HasForeignKey(d => d.FolderLinkId)
                    .HasConstraintName("FK_SharedFolders_FolderLinks");

                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_SharedFolders_Users");
            });

            modelBuilder.Entity<Tariff>(entity =>
            {
                entity.Property(e => e.UploadLimitName).HasMaxLength(30);
                entity.Property(e => e.Price).HasColumnType("money");
            });

            modelBuilder.Entity<TariffOfUser>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                entity.Property(e => e.EndAt).HasColumnType("timestamp");

                entity.HasOne(d => d.Tariff).WithMany(p => p.TariffsOfUsers)
                    .HasForeignKey(d => d.TariffId)
                    .HasConstraintName("FK_TariffsOfUsers_Tariffs");

                entity.HasOne(d => d.User).WithMany(p => p.TariffsOfUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_TariffsOfUsers_Users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.IsImageExists).HasDefaultValue(false);
                entity.Property(e => e.About).HasColumnType("text");
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                entity.Property(e => e.FirstName).HasMaxLength(20);
                entity.Property(e => e.Password).HasMaxLength(20);
                entity.Property(e => e.SecondName).HasMaxLength(20);
                entity.Property(e => e.Username).HasMaxLength(20);
                entity.Property(e => e.Birthday).HasColumnType("timestamp");

                entity.HasOne(d => d.PrimaryEmail).WithMany(p => p.Users)
                    .HasForeignKey(d => d.PrimaryEmailId)
                    .HasConstraintName("FK_Users_Emails");
            });

            modelBuilder.Entity<ViewOfFile>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");

                entity.HasOne(d => d.File).WithMany(p => p.ViewsOfFiles)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ViewsOfFiles_Files");

                entity.HasOne(d => d.User).WithMany(p => p.ViewsOfFiles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ViewsOfFiles_Users");
            });

            modelBuilder.Entity<ViewOfFolder>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");

                entity.HasOne(d => d.Folder).WithMany(p => p.ViewsOfFolders)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ViewsOfFolders_Folders");

                entity.HasOne(d => d.User).WithMany(p => p.ViewsOfFolders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ViewsOfFolders_Users");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Text).HasColumnType("text");
                entity.Property(e => e.Answer).HasColumnType("text");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                entity.Property(e => e.RespondedAt).HasColumnType("timestamp");

                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Questions_Users");
            });
        }
    }
}
