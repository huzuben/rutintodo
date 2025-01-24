using System.ComponentModel.DataAnnotations;

namespace todolist.Models
{
    public class Kullanici
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Soyad { get; set; } = string.Empty;

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public bool AktifMi { get; set; } = true;

        // Kullanıcının görevleri ile ilişki
        public virtual ICollection<Gorev> Gorevler { get; set; } = new List<Gorev>();
    }
} 