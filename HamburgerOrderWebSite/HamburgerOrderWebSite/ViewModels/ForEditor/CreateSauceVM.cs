using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HamburgerOrderWebSite.ViewModels.ForEditor
{
    public class CreateSauceVM
    {
        [Display(Name = "Sos Adı")]
        [Required(ErrorMessage = "Ad kısmı boş bırakılamaz")]
        public string SauceName { get; set; }

        [Display(Name = "Sos Fiyatı")]
        [Required(ErrorMessage = "Fiyat kısmı boş bırakılamaz")]
        public decimal Price { get; set; }
    }
}
