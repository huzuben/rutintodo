using Microsoft.EntityFrameworkCore;
using todolist.Data;
using todolist.Models;

namespace todolist.Services
{
    public class BildirimServisi : IBildirimServisi
    {
        private readonly VeriTabaniContext _veriTabani;

        public BildirimServisi(VeriTabaniContext veriTabani)
        {
            _veriTabani = veriTabani;
        }

        public async Task BildirimOlustur(string mesaj, string? detay, BildirimTipi tip, int? gorevId = null)
        {
            var bildirim = new Bildirim
            {
                Mesaj = mesaj,
                Detay = detay,
                Tip = tip,
                GorevId = gorevId,
                OlusturulmaTarihi = DateTime.Now,
                Okundu = false
            };

            await _veriTabani.Bildirimler.AddAsync(bildirim);
            await _veriTabani.SaveChangesAsync();
        }

        public async Task<List<Bildirim>> OkunmamisBildirimleriGetir()
        {
            return await _veriTabani.Bildirimler
                .Include(b => b.Gorev)
                .Where(b => !b.Okundu)
                .OrderByDescending(b => b.OlusturulmaTarihi)
                .ToListAsync();
        }

        public async Task<int> OkunmamisBildirimSayisiniGetir()
        {
            return await _veriTabani.Bildirimler
                .Where(b => !b.Okundu)
                .CountAsync();
        }

        public async Task BildirimOkunduOlarakIsaretle(int bildirimId)
        {
            var bildirim = await _veriTabani.Bildirimler.FindAsync(bildirimId);
            if (bildirim != null)
            {
                bildirim.Okundu = true;
                await _veriTabani.SaveChangesAsync();
            }
        }

        public async Task TumBildirimleriOkunduOlarakIsaretle()
        {
            var bildirimler = await _veriTabani.Bildirimler
                .Where(b => !b.Okundu)
                .ToListAsync();

            foreach (var bildirim in bildirimler)
            {
                bildirim.Okundu = true;
            }

            await _veriTabani.SaveChangesAsync();
        }

        public async Task<List<Bildirim>> SonBildirimleriGetir(int adet = 10)
        {
            return await _veriTabani.Bildirimler
                .Include(b => b.Gorev)
                .OrderByDescending(b => b.OlusturulmaTarihi)
                .Take(adet)
                .ToListAsync();
        }

        public async Task EskiBildirimleriTemizle(int gunSayisi = 30)
        {
            var eskiTarih = DateTime.Now.AddDays(-gunSayisi);
            var eskiBildirimler = await _veriTabani.Bildirimler
                .Where(b => b.OlusturulmaTarihi < eskiTarih)
                .ToListAsync();

            _veriTabani.Bildirimler.RemoveRange(eskiBildirimler);
            await _veriTabani.SaveChangesAsync();
        }
    }
} 