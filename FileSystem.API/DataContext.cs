using Microsoft.EntityFrameworkCore;

namespace FileSystem.API
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public virtual DbSet<Folder> Folders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folder>()
                .HasOne(i => i.Parent)
                .WithMany()
                .HasForeignKey(i => i.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Folder>()
                .HasMany(i => i.Children)
                .WithOne(parent => parent.Parent)
                .HasForeignKey(child => child.ParentId)
                .OnDelete(DeleteBehavior.NoAction);


            base.OnModelCreating(modelBuilder);
        }
    }
}
