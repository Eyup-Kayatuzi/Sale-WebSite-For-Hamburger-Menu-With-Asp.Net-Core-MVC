using System.ComponentModel.DataAnnotations;

namespace HamburgerOrderWebSite.ViewModels.ForEditor
{
    public class MenuUpdateVM
    {
        [Display(Name ="Menü Adı")]
        [Required(ErrorMessage ="Menü adı kısmı boş bırakılamaz")]
        public string MenuName { get; set; }
        [Display(Name = "Menü Fiyatı")]
        [Required(ErrorMessage = "Menü fiyat kısmı boş bırakılamaz")]
        public decimal Price { get; set; }
        public string PicturePath { get; set; }
        public IFormFile NewPicturePath { get; set; }
    }
}
