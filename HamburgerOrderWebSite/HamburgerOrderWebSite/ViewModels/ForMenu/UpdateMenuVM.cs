using HamburgerOrderWebSite.Enums;
using HamburgerOrderWebSite.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HamburgerOrderWebSite.ViewModels.ForMenu
{
    public class UpdateMenuVM
    {
        [Display(Name = "Menü Adedi")]
        public int Quantity { get; set; }

        public List<SauceSelectionVM> SauceSelections { get; set; } = new List<SauceSelectionVM>();

        [Display(Name = "Menü Boyutu")]
        public MenuSize MenuSizes { get; set; }

        public int OrderId { get; set; }
    }
}
