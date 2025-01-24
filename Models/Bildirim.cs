using System.ComponentModel.DataAnnotations;

namespace todolist.Models
{
    public class Bildirim
    {
        public int Id { get; set; }

        [Required]
        public string Mesaj { get; set; } = string.Empty;

        public string? Detay { get; set; }

        public BildirimTipi Tip { get; set; }

        public bool Okundu { get; set; }

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        public int? GorevId { get; set; }
        public Gorev? Gorev { get; set; }
    }

    public enum BildirimTipi
    {
        Hatirlatma,
        GorevTamamlandi,
        GorevGecikti,
        YeniGorev
    }
} 