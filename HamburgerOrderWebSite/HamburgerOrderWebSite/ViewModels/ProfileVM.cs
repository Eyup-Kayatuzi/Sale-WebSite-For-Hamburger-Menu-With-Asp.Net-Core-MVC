using System.ComponentModel.DataAnnotations;

namespace HamburgerOrderWebSite.ViewModels
{
    public class ProfileVM
    {
        [Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mail Boş Bırakılamaz")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Girilen mail adresi geçersizdir")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon kısmı boş bırakılmaz")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Lütfen telefon numaranızı başına 0 koymadan 10 haneli şekilde yazınız")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Adres kısmı boş bırakılmaz")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }


    }
}
