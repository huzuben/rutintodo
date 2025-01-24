using todolist.Models;

namespace todolist.Services
{
    public interface IBildirimServisi
    {
        Task BildirimOlustur(string mesaj, string? detay, BildirimTipi tip, int? gorevId = null);
        Task<List<Bildirim>> OkunmamisBildirimleriGetir();
        Task<int> OkunmamisBildirimSayisiniGetir();
        Task BildirimOkunduOlarakIsaretle(int bildirimId);
        Task TumBildirimleriOkunduOlarakIsaretle();
        Task<List<Bildirim>> SonBildirimleriGetir(int adet = 10);
        Task EskiBildirimleriTemizle(int gunSayisi = 30);
    }
} 