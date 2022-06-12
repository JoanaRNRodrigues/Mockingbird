using Microsoft.EntityFrameworkCore;
using MockingBird.Models;

namespace MockingBird.Models
{
    public class MockingBirdContext : DbContext
    {
        public DbSet<Chirp> Chirps { get; set; }
        public DbSet<User> Users { get; set; }

        public MockingBirdContext(DbContextOptions<MockingBirdContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //change password according to server that is used
            optionsBuilder.UseMySQL("server=localhost;database=mockingbird;user=root;password=thJJHnbdVAXB6DL9l696S");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chirp>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.HasOne(d => d.User);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.UserName).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.ConfirmPassword).IsRequired();
                entity.Property(e => e.Email).IsRequired();
            });
        }
    }
}
