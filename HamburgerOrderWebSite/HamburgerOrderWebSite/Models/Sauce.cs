using System.ComponentModel.DataAnnotations;

namespace HamburgerOrderWebSite.Models
{
    public class Sauce
    {
        public int Id { get; set; }
        [Display(Name ="Sos Adı")]
        public string SauceName { get; set; }
        [Display(Name ="Fiyat")]
        public decimal Price { get; set; }
        public virtual List<SauceOrder> SauceOrders { get; set; } = new List<SauceOrder>();
    }
}
