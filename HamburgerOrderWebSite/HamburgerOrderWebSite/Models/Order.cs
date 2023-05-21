using HamburgerOrderWebSite.Enums;

namespace HamburgerOrderWebSite.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public MenuSize MenuSize { get; set; }
        public bool IsItCompleted { get; set; } = false;
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public virtual List<SauceOrder> SauceOrders { get; set; } = new List<SauceOrder>();
        public virtual List<OrderPrice> OrderPrices { get; set; } = new List<OrderPrice>();
    }
}
