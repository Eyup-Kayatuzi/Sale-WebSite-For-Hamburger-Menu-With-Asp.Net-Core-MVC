namespace HamburgerOrderWebSite.ViewModels.ForMenu
{
    public class SauceSelectionVM
    {
        public int SauceId { get; set; }
        //public bool IsSelected { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
    }
}
