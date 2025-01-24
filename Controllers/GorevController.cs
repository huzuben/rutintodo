using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todolist.Data;
using todolist.Models;
using todolist.Services;

namespace todolist.Controllers
{
    [Authorize]
    public class GorevController : Controller
    {
        private readonly VeriTabaniContext _veriTabani;
        private readonly IBildirimServisi _bildirimServisi;

        public GorevController(VeriTabaniContext veriTabani, IBildirimServisi bildirimServisi)
        {
            _veriTabani = veriTabani;
            _bildirimServisi = bildirimServisi;
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            var gorevler = await _veriTabani.Gorevler
                .Where(g => g.KullaniciId == kullaniciId)
                .OrderByDescending(g => g.OlusturulmaTarihi)
                .ToListAsync();
            return View(gorevler);
        }

        public async Task<IActionResult> Istatistikler()
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            var gorevler = await _veriTabani.Gorevler
                .Where(g => g.KullaniciId == kullaniciId)
                .ToListAsync();

            var istatistik = new GorevIstatistik
            {
                ToplamGorev = gorevler.Count,
                TamamlananGorev = gorevler.Count(t => t.Tamamlandi),
                TamamlanmaOrani = gorevler.Any() ? (double)gorevler.Count(t => t.Tamamlandi) / gorevler.Count * 100 : 0,
                GecikmisTamamlanmayanGorev = gorevler.Count(t => !t.Tamamlandi && t.SonTarih.HasValue && t.SonTarih.Value < DateTime.Now),
                OncelikDagilimi = new Dictionary<string, int>
                {
                    { "Yüksek", gorevler.Count(t => t.OncelikDurumu == Oncelik.Yüksek) },
                    { "Orta", gorevler.Count(t => t.OncelikDurumu == Oncelik.Orta) },
                    { "Düşük", gorevler.Count(t => t.OncelikDurumu == Oncelik.Düşük) }
                }
            };

            return View(istatistik);
        }

        public IActionResult Olustur()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Olustur([Bind("Baslik,Aciklama,OncelikDurumu,SonTarih")] Gorev gorev)
        {
            if (ModelState.IsValid)
            {
                var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (kullaniciIdClaim == null)
                {
                    return RedirectToAction("Giris", "Hesap");
                }

                var kullaniciId = int.Parse(kullaniciIdClaim.Value);
                gorev.KullaniciId = kullaniciId;
                gorev.OlusturulmaTarihi = DateTime.Now;
                gorev.Tamamlandi = false;

                _veriTabani.Add(gorev);
                await _veriTabani.SaveChangesAsync();

                await _bildirimServisi.BildirimOlustur(
                    "Yeni görev oluşturuldu",
                    $"'{gorev.Baslik}' başlıklı yeni bir görev oluşturuldu.",
                    BildirimTipi.YeniGorev,
                    gorev.Id);

                return RedirectToAction(nameof(Index));
            }
            return View(gorev);
        }

        public async Task<IActionResult> Duzenle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            var gorev = await _veriTabani.Gorevler
                .FirstOrDefaultAsync(g => g.Id == id && g.KullaniciId == kullaniciId);

            if (gorev == null)
            {
                return NotFound();
            }

            return View(gorev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, [Bind("Id,Baslik,Aciklama,OncelikDurumu,SonTarih,Tamamlandi")] Gorev gorev)
        {
            if (id != gorev.Id)
            {
                return NotFound();
            }

            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            var mevcutGorev = await _veriTabani.Gorevler
                .FirstOrDefaultAsync(g => g.Id == id && g.KullaniciId == kullaniciId);

            if (mevcutGorev == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    mevcutGorev.Baslik = gorev.Baslik;
                    mevcutGorev.Aciklama = gorev.Aciklama;
                    mevcutGorev.OncelikDurumu = gorev.OncelikDurumu;
                    mevcutGorev.SonTarih = gorev.SonTarih;

                    if (!mevcutGorev.Tamamlandi && gorev.Tamamlandi)
                    {
                        await _bildirimServisi.BildirimOlustur(
                            "Görev tamamlandı",
                            $"'{gorev.Baslik}' başlıklı görev tamamlandı olarak işaretlendi.",
                            BildirimTipi.GorevTamamlandi,
                            gorev.Id);
                    }

                    mevcutGorev.Tamamlandi = gorev.Tamamlandi;

                    await _veriTabani.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GorevExists(gorev.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gorev);
        }

        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(int id)
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            var gorev = await _veriTabani.Gorevler
                .FirstOrDefaultAsync(g => g.Id == id && g.KullaniciId == kullaniciId);

            if (gorev == null)
            {
                return NotFound();
            }

            gorev.Tamamlandi = !gorev.Tamamlandi;

            if (gorev.Tamamlandi)
            {
                await _bildirimServisi.BildirimOlustur(
                    "Görev tamamlandı",
                    $"'{gorev.Baslik}' başlıklı görev tamamlandı olarak işaretlendi.",
                    BildirimTipi.GorevTamamlandi,
                    gorev.Id);
            }

            await _veriTabani.SaveChangesAsync();
            return Ok(new { success = true, completed = gorev.Tamamlandi });
        }

        [HttpPost]
        public async Task<IActionResult> Sil(int id)
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            var gorev = await _veriTabani.Gorevler
                .FirstOrDefaultAsync(g => g.Id == id && g.KullaniciId == kullaniciId);

            if (gorev == null)
            {
                return NotFound();
            }

            _veriTabani.Gorevler.Remove(gorev);
            await _veriTabani.SaveChangesAsync();
            return Ok(new { success = true });
        }

        private bool GorevExists(int id)
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return false;
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            return _veriTabani.Gorevler.Any(e => e.Id == id && e.KullaniciId == kullaniciId);
        }

        public async Task<IActionResult> Dashboard()
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (kullaniciIdClaim == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullaniciId = int.Parse(kullaniciIdClaim.Value);
            var model = new DashboardViewModel
            {
                BekleyenGorevler = await _veriTabani.Gorevler
                    .Where(g => !g.Tamamlandi && g.KullaniciId == kullaniciId)
                    .OrderBy(g => g.SonTarih)
                    .Take(5)
                    .ToListAsync(),

                DevamEdenGorevler = await _veriTabani.Gorevler
                    .Where(g => !g.Tamamlandi && g.KullaniciId == kullaniciId && g.SonTarih > DateTime.Now)
                    .OrderBy(g => g.SonTarih)
                    .Take(5)
                    .ToListAsync(),

                TamamlananGorevler = await _veriTabani.Gorevler
                    .Where(g => g.Tamamlandi && g.KullaniciId == kullaniciId)
                    .OrderByDescending(g => g.OlusturulmaTarihi)
                    .Take(5)
                    .ToListAsync(),

                GecikmisGorevler = await _veriTabani.Gorevler
                    .Where(g => !g.Tamamlandi && g.KullaniciId == kullaniciId && g.SonTarih < DateTime.Now)
                    .OrderBy(g => g.SonTarih)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }
} 