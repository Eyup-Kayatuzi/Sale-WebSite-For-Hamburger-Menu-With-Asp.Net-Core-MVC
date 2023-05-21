namespace HamburgerOrderWebSite.Models
{
    public class SauceOrder
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int SauceId { get; set; }
        public virtual Sauce Sauce { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

    }
}
