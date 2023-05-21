using HamburgerOrderWebSite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HamburgerOrderWebSite.Context
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> option) : base(option)
        {

        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Sauce> Sauces { get; set; }
        public DbSet<SauceOrder> SauceOrders { get; set; }
        public DbSet<OrderPrice> OrderPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SauceOrder>()
            .HasIndex(os => new { os.OrderId, os.SauceId })
            .IsUnique();
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 1, MenuName = "BigKing", PicturePath = "d07e0859-4dca-4d4f-9cd3-9bfd4f82e012-BigKingIcin.jpeg", Price = 110 }
                );
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 2, MenuName = "SteakHouse", PicturePath = "steakhouseburgersandvic.jpg", Price = 100 }
                );
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 3, MenuName = "BigKing Chicken", PicturePath = "bigKing-chicken-burger-king-big.png", Price = 95 }
                );
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 4, MenuName = "Whopper", PicturePath = "whopper-eyp.png", Price = 80 }
                );
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 5, MenuName = "Whopper Jr.", PicturePath = "a742932c-108e-489d-80da-5c4839a5478e-dddddd.jpg", Price = 70 }
                );
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 6, MenuName = "Chicken Royal", PicturePath = "chicken-royale-eyp.png", Price = 65 }
                );

            modelBuilder.Entity<Sauce>().HasData(
                new Sauce { Id = 1, SauceName = "Ketçap", Price = 3 }, new Sauce { Id = 2, SauceName = "Mayonez", Price = 3 }, new Sauce { Id = 3, SauceName = "Hardal", Price = 4 }, new Sauce { Id = 4, SauceName = "BBQ", Price = 2 }, new Sauce { Id = 5, SauceName = "Ranch", Price = 2 }, new Sauce { Id = 6, SauceName = "Buffalo", Price = 3 }
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Name = "Yönetici",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole()
                {
                    Name = "Editör",
                    NormalizedName = "EDITOR",
                },
                new IdentityRole()
                {
                    Name = "Normal Kullanıcı",
                    NormalizedName = "USER",
                }
            );

            base.OnModelCreating(modelBuilder);
        }

    }
}
