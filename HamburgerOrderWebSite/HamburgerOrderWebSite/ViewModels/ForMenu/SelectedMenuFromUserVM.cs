using HamburgerOrderWebSite.Enums;

namespace HamburgerOrderWebSite.ViewModels.ForMenu
{
    public class SelectedMenuFromUserVM
    {
        public string MenuName { get; set; }
        public int Quantity { get; set; }
        public List<SauceSelectionVM> SauceSelections { get; set; } = new List<SauceSelectionVM>();
        public MenuSize MenuSizes { get; set; }
    }
}
