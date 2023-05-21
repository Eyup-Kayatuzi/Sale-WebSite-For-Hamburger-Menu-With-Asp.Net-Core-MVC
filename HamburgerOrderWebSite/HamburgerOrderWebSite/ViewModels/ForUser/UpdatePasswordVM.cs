using System.ComponentModel.DataAnnotations;

namespace HamburgerOrderWebSite.ViewModels.ForUser
{
    public class UpdatePasswordVM
    {
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [MinLength(3, ErrorMessage = "Minimum 3 karakter olması gerek")]
        [DataType(DataType.Password)]
        [Display(Name ="Mevcut Şifre")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [MinLength(3, ErrorMessage = "Minimum 3 karakter olması gerek")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Bu Alanı Boş Bırakılamaz")]
        [Compare("NewPassword", ErrorMessage = "Şifre eşleşmemektedir")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifreyi Tekrarla")]
        public string RepeateNewPassword { get; set; }
    }
}
