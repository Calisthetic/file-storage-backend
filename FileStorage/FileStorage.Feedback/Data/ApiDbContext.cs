using FileStorage.Feedback.Models;
using FileStorage.Feedback.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Feedback.Data;

public partial class ApiDbContext : DbContext
{
    private IConfiguration _configuration;
    public ApiDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual DbSet<AppPart> AppParts { get; set; }

    public virtual DbSet<Bug> Bugs { get; set; }

    public virtual DbSet<BugStatus> BugStatuses { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_configuration.GetSection("ConnectionStrings:DatabaseConnection").Value!);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bug>(entity =>
        {
            entity.Property(e => e.Text).HasMaxLength(50);

            entity.HasOne(d => d.AppPart).WithMany(p => p.Bugs)
                .HasForeignKey(d => d.AppPartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bugs_AppParts");

            entity.HasOne(d => d.BugStatus).WithMany(p => p.Bugs)
                .HasForeignKey(d => d.BugStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bugs_BugStatuses");
        });

        modelBuilder.Entity<BugStatus>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.RespondedAt).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
