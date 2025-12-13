using System.ComponentModel.DataAnnotations;

namespace LmsProje.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Display(Name = "E-Posta (Değiştirilemez)")]
        public string Email { get; set; }

        [Display(Name = "Şehir")]
        public string? City { get; set; }

        [Display(Name = "Mevcut Profil Resmi")]
        public string? CurrentImage { get; set; }

        [Display(Name = "Yeni Profil Resmi Yükle")]
        public IFormFile? ProfileImage { get; set; }
    }
}
