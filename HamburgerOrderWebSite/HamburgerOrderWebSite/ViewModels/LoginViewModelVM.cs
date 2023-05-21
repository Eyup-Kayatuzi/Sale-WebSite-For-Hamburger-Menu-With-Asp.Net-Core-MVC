using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HamburgerOrderWebSite.ViewModels
{
    public class LoginViewModelVM
    {
        [Required(ErrorMessage ="Kullanıcı adı kısmı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Şifre kısmı boş bırakılamaz")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
