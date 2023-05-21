using Microsoft.AspNetCore.Identity;

namespace HamburgerOrderWebSite.Models
{
    public class AppUser : IdentityUser
    {
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual List<Order> Orders { get; set; } = new List<Order>();
    }
}
