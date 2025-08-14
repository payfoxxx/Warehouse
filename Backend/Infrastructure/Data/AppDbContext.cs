using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Converters;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        public DbSet<Balance> Balances { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<MeasureUnit> MeasureUnits { get; set; }
        public DbSet<ReceiptDocument> ReceiptDocuments { get; set; }
        public DbSet<ReceiptResource> ReceiptResources { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ShipmentDocument> ShipmentDocuments { get; set; }
        public DbSet<ShipmentResource> ShipmentResources { get; set; }


        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateTime>()
                .HaveConversion<UtcDateTimeConverter>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Balance>()
                .HasIndex(b => new { b.ResourceId, b.MeasureUnitId })
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<MeasureUnit>()
                .HasIndex(mu => mu.Name)
                .IsUnique();

            modelBuilder.Entity<Resource>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<ShipmentDocument>()
                .HasIndex(sd => sd.Num)
                .IsUnique();

            modelBuilder.Entity<ReceiptDocument>()
                .HasIndex(rd => rd.Num)
                .IsUnique();
            
            modelBuilder.Entity<ReceiptDocument>(entity =>
            {
                entity.Property(e => e.Date)
                    .HasColumnType("timestamp with time zone")
                    .HasConversion<UtcDateTimeConverter>();
            });
        }
    }
}
