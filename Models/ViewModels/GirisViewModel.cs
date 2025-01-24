using System.ComponentModel.DataAnnotations;

namespace todolist.Models.ViewModels
{
    public class GirisViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = string.Empty;

        [Display(Name = "Beni Hatırla")]
        public bool BeniHatirla { get; set; }
    }
} 