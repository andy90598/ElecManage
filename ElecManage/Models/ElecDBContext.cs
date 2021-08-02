using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ElecManage.Models
{
    public partial class ElecDBContext : DbContext
    {
        public ElecDBContext()
        {
        }

        public ElecDBContext(DbContextOptions<ElecDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Electricity> Electricities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.DeviceId).HasColumnName("DeviceID");

                entity.Property(e => e.DeviceKWh).HasColumnName("Device_kWh");

                entity.Property(e => e.DeviceName).IsRequired();
            });

            modelBuilder.Entity<Electricity>(entity =>
            {
                entity.HasKey(e => e.ElecSn);

                entity.ToTable("Electricity");

                entity.HasIndex(e => e.ElecSn, "IX_Electricity_Elec_sn")
                    .IsUnique();

                entity.Property(e => e.ElecSn).HasColumnName("Elec_sn");

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasColumnName("DeviceID");

                entity.Property(e => e.Time).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
