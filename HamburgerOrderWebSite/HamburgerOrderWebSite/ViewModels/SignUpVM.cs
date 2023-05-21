using System.ComponentModel.DataAnnotations;

namespace HamburgerOrderWebSite.ViewModels
{
    public class SignUpVM
    {
        [Required(ErrorMessage ="Kullanıcı Adı Boş Bırakılamaz")]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mail Boş Bırakılamaz")]
        [DataType(DataType.EmailAddress, ErrorMessage ="Girilen mail adresi geçersizdir")]
        [Display(Name = "Mail Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [MinLength(3, ErrorMessage = "Minimum 3 karakter olması gerek")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bu Alanı Boş Bırakılamaz")]
        [Compare("Password", ErrorMessage ="Şifre eşleşmemektedir")]
        [DataType(DataType.Password)]
        public string RepeatePassword { get; set; }

        [Required(ErrorMessage = "Adres kısmı boş bırakılmaz")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Telefon kısmı boş bırakılmaz")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Lütfen telefon numaranızı başına 0 koymadan 10 haneli şekilde yazınız")]
        public string PhoneNumber { get; set; }
    }
}
