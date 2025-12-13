using System.ComponentModel.DataAnnotations;

namespace LmsProje.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta giriniz.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre giriniz.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}