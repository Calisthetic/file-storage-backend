using FileStorage.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Data
{
    public class ApiDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        
        public ApiDbContext(DbContextOptions<ApiDbContext> options) :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Email>(entity =>
            {
                entity.HasOne(x => x.User)
                    .WithMany(d => d.Emails)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
