using HamburgerOrderWebSite.Enums;
using HamburgerOrderWebSite.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HamburgerOrderWebSite.ViewModels.ForMenu
{
    public class SelectedMenuVM
    {
        public string MenuName { get; set; }
        public decimal Price { get; set; }
        public string PicturePath { get; set; }

        [Display(Name = "Menü Adedi")]
        public int Quantity { get; set; }

        public List<Sauce> Sauces { get; set; } = new List<Sauce>();

        public List<SauceSelectionVM> SauceSelections { get; set; } = new List<SauceSelectionVM>();

        [Display(Name = "Menü Boyutu")]
        public MenuSize MenuSizes { get; set; }

    }
}
