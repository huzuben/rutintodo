using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todolist.Models
{
    public class Gorev
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur")]
        public string Aciklama { get; set; } = string.Empty;

        public bool Tamamlandi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        public DateTime? SonTarih { get; set; }

        public Oncelik OncelikDurumu { get; set; }

        // Kullanıcı ile ilişki
        [Required]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public virtual Kullanici? Kullanici { get; set; }
    }

    public enum Oncelik
    {
        Düşük,
        Orta,
        Yüksek
    }
} 