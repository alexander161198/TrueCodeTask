using Microsoft.EntityFrameworkCore;
using SharedModels.EntityModels;

namespace SharedModels
{
    public class TrueCodeContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<UserCurrency> UserCurrencies { get; set; }

        public TrueCodeContext(DbContextOptions<TrueCodeContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired();
                entity.Property(u => u.Password).IsRequired();
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("currency");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired();
                entity.Property(c => c.Rate).HasColumnType("decimal(18,4)");
            });

            modelBuilder.Entity<UserCurrency>(entity =>
            {
                entity.ToTable("user_currency");
                entity.HasKey(uc => new { uc.UserId, uc.CurrencyId });

                entity.HasOne(uc => uc.User)
                    .WithMany(u => u.Currencies)
                    .HasForeignKey(uc => uc.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uc => uc.Currency)
                    .WithMany(c => c.Users)
                    .HasForeignKey(uc => uc.CurrencyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
