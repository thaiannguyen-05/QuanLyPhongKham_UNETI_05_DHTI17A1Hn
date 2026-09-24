using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;
namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(t => t.Username)
                .IsUnique();

            modelBuilder.Entity<Specialty>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .Property(b => b.ConsultationFee)
                .HasPrecision(18, 2);
        }
    }
}
