using System.ComponentModel.DataAnnotations;

namespace todolist.Models.ViewModels
{
    public class SifreSifirlamaViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [Display(Name = "Yeni Şifre")]
        [DataType(DataType.Password)]
        public string YeniSifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
        [Compare("YeniSifre", ErrorMessage = "Şifreler eşleşmiyor")]
        [Display(Name = "Yeni Şifre Tekrar")]
        [DataType(DataType.Password)]
        public string YeniSifreTekrar { get; set; } = string.Empty;
    }
} 