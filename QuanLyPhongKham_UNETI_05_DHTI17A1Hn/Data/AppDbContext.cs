using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;
namespace QuanLyPhongKham_UNETI05_TI17A1Hn.Data
{
    
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }

        public DbSet<ChuyenKhoa> ChuyenKhoas { get; set; }

        public DbSet<BacSi> BacSis { get; set; }

        public DbSet<BenhNhan> BenhNhans { get; set; }

        public DbSet<LichKham> LichKhams { get; set; }

        public DbSet<PhieuDangKyKham> PhieuDangKyKhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<ChuyenKhoa>()
                .HasIndex(c => c.TenChuyenKhoa)
                .IsUnique();
        }
    }
}