using Microsoft.AspNetCore.Mvc;
using todolist.Services;

namespace todolist.Controllers
{
    public class BildirimController : Controller
    {
        private readonly IBildirimServisi _bildirimServisi;

        public BildirimController(IBildirimServisi bildirimServisi)
        {
            _bildirimServisi = bildirimServisi;
        }

        public async Task<IActionResult> Index()
        {
            var bildirimler = await _bildirimServisi.SonBildirimleriGetir(50);
            return View(bildirimler);
        }

        [HttpGet]
        public async Task<IActionResult> OkunmamisBildirimler()
        {
            var bildirimler = await _bildirimServisi.OkunmamisBildirimleriGetir();
            return Json(bildirimler);
        }

        [HttpGet]
        public async Task<IActionResult> OkunmamisBildirimSayisi()
        {
            var sayi = await _bildirimServisi.OkunmamisBildirimSayisiniGetir();
            return Json(new { sayi });
        }

        [HttpPost]
        public async Task<IActionResult> BildirimOkundu(int id)
        {
            await _bildirimServisi.BildirimOkunduOlarakIsaretle(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> TumBildirimleriOkunduOlarakIsaretle()
        {
            await _bildirimServisi.TumBildirimleriOkunduOlarakIsaretle();
            return Json(new { success = true });
        }
    }
} 