namespace HamburgerOrderWebSite.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public decimal Price { get; set; }
        public string PicturePath { get; set; }
        public virtual List<Order> Orders { get; set; } = new List<Order>();
    }
}
