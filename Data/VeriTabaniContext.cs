using Microsoft.EntityFrameworkCore;
using todolist.Models;

namespace todolist.Data
{
    public class VeriTabaniContext : DbContext
    {
        public VeriTabaniContext(DbContextOptions<VeriTabaniContext> options)
            : base(options)
        {
        }

        public DbSet<Gorev> Gorevler { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kullanıcı - Görev ilişkisi
            modelBuilder.Entity<Gorev>()
                .HasOne<Kullanici>()
                .WithMany(k => k.Gorevler)
                .HasForeignKey(g => g.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 