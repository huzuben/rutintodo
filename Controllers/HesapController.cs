using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todolist.Data;
using todolist.Models;
using todolist.Models.ViewModels;

namespace todolist.Controllers
{
    public class HesapController : Controller
    {
        private readonly VeriTabaniContext _context;

        public HesapController(VeriTabaniContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Giris()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Gorev");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(GirisViewModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.KullaniciAdi == model.KullaniciAdi && k.AktifMi);

                if (kullanici != null && BCrypt.Net.BCrypt.Verify(model.Sifre, kullanici.Sifre))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                        new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                    {
                        IsPersistent = model.BeniHatirla,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                    });

                    return RedirectToAction("Index", "Gorev");
                }

                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Kayit()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Gorev");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kayit(KayitViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == model.KullaniciAdi))
                {
                    ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten kullanılıyor");
                    return View(model);
                }

                if (await _context.Kullanicilar.AnyAsync(k => k.Eposta == model.Eposta))
                {
                    ModelState.AddModelError("Eposta", "Bu e-posta adresi zaten kullanılıyor");
                    return View(model);
                }

                var kullanici = new Kullanici
                {
                    KullaniciAdi = model.KullaniciAdi,
                    Eposta = model.Eposta,
                    Sifre = BCrypt.Net.BCrypt.HashPassword(model.Sifre),
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    KayitTarihi = DateTime.Now,
                    AktifMi = true
                };

                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Giris));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Giris", "Hesap");
        }

        [HttpGet]
        public IActionResult SifreSifirla()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Gorev");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreSifirla(SifreSifirlamaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.KullaniciAdi == model.KullaniciAdi && 
                                            k.Eposta == model.Eposta && 
                                            k.AktifMi);

                if (kullanici != null)
                {
                    try
                    {
                        kullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(model.YeniSifre);
                        await _context.SaveChangesAsync();

                        TempData["BasariliMesaj"] = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.";
                        return RedirectToAction(nameof(Giris));
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Şifre güncellenirken bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.");
                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Girdiğiniz kullanıcı adı veya e-posta adresi sistemde bulunamadı.");
            }
            return View(model);
        }
    }
} 