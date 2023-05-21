using HamburgerOrderWebSite.Enums;
using System.ComponentModel.DataAnnotations;

namespace HamburgerOrderWebSite.ViewModels.ForUser
{
    public class OldOrdersVM
    {
        [Display(Name ="Siapariş Adı")]
        public string Name { get; set; }
        [Display(Name = "Adet")]
        public int Quantity { get; set; }
        [Display(Name = "Boyut")]
        public MenuSize MenuSize { get; set; }

        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        public string PicturePath { get; set; }
        public int OrderId { get; set; }


    }
}
