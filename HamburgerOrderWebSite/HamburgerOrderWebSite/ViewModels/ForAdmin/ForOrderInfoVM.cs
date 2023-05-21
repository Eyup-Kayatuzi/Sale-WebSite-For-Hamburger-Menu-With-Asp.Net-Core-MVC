namespace HamburgerOrderWebSite.ViewModels.ForAdmin
{
    public class ForOrderInfoVM
    {
        public List<string> Menus { get; set; } = new List<string>();
        public List<decimal> Prices { get; set; } = new List<decimal>();
        public List<string> SauceNames { get; set; } = new List<string>();
        public List<decimal> SauceQuantity { get; set; } = new List<decimal>();
    }
}
