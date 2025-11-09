using Microsoft.EntityFrameworkCore;
using PrimeVideoPaymentSimulator.Models; // Usar el namespace del modelo

namespace PrimeVideoPaymentSimulator.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CardModel> ValidatedCards { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}