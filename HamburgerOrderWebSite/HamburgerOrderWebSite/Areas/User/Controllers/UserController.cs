using HamburgerOrderWebSite.Context;
using HamburgerOrderWebSite.Enums;
using HamburgerOrderWebSite.Models;
using HamburgerOrderWebSite.ViewModels;
using HamburgerOrderWebSite.ViewModels.ForMenu;
using HamburgerOrderWebSite.ViewModels.ForUser;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Claims;

namespace HamburgerOrderWebSite.Areas.User.Controllers
{
    [Authorize]
    [Area("User")]
    public class UserController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly ApplicationContext _db;

        public UserController(ApplicationContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _passwordHasher = passwordHasher;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ValidateBox()
        {
            var targetOrders = await _db.Orders.Where(x => x.IsItCompleted == false).ToListAsync();
            foreach (var item in targetOrders)
            {
                item.IsItCompleted = true;
                _db.Orders.Update(item);   
            }
            await _db.SaveChangesAsync();
            TempData["SuccessOrder"] = "Şiparişiniz Başarıyla Tamamlanmıştır";
            return RedirectToAction("MenuList", "MainPage", new { area = "" });
        }
        public async Task<IActionResult> ProfileDetail()
        {
            ListingAllBox();
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var targetUser = await _userManager.FindByIdAsync(userId);
            ProfileVM profileVM = new ProfileVM() { Address = targetUser.Address, Email = targetUser.Email, PhoneNumber = targetUser.PhoneNumber, UserName = targetUser.UserName };
            return View(profileVM);
        }
        [HttpPost]
        public async Task<IActionResult> ProfileDetail(ProfileVM profileVM)
        {
            ListingAllBox();
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var targetUser = await _userManager.FindByIdAsync(userId);
            targetUser.PhoneNumber = profileVM.PhoneNumber;
            targetUser.UserName = profileVM.UserName;
            targetUser.Address = profileVM.Address;
            targetUser.NormalizedUserName = profileVM.UserName.ToUpper();
            _db.AppUsers.Update(targetUser);
            await _db.SaveChangesAsync();
            return RedirectToAction("LogOff", "MainPage", new { area = "" });
        }
        public async Task<IActionResult> DeleteProfile()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var targetUser = await _userManager.FindByIdAsync(userId);
            _db.AppUsers.Remove(targetUser);
            await _db.SaveChangesAsync();
            return RedirectToAction("LogOff", "MainPage", new { area = "" });
        }

        public IActionResult UpdatePassword()
        {
            ListingAllBox();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordVM updatePasswordVM)
        {
            ListingAllBox();
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var targetUser = await _userManager.FindByIdAsync(userId);
            if(await _userManager.CheckPasswordAsync(targetUser, updatePasswordVM.CurrentPassword) && !string.IsNullOrEmpty(targetUser.PasswordHash))
            {
                targetUser.PasswordHash = _passwordHasher.HashPassword(targetUser, updatePasswordVM.NewPassword);
                IdentityResult result = await _userManager.UpdateAsync(targetUser);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                    return RedirectToAction("LogOff", "MainPage", new { area = "" });
                }

            }
            ModelState.AddModelError("", "Şifre Yenileme Sırasında Bir Hata Oluştu");
            return View();
        }


        public IActionResult SepeteEkle(string selectedMenuName)
        {
            ListingAllBox();

            var targetMenu = _db.Menus.Where(x => x.MenuName == selectedMenuName);
            var targetSauces = _db.Sauces.ToList();
            SelectedMenuVM selectedMenu = targetMenu.Select(x => new SelectedMenuVM() { MenuName = x.MenuName, Price = x.Price, PicturePath = x.PicturePath }).FirstOrDefault();
            selectedMenu.Sauces = targetSauces;
            foreach (var sauce in targetSauces)
            {
                selectedMenu.SauceSelections.Add(new SauceSelectionVM { SauceId = sauce.Id/*, IsSelected = false*/, Price = sauce.Price });
            }
            return View(selectedMenu);
        }

        [HttpPost]
        public async Task<IActionResult> SepeteEkle(SelectedMenuFromUserVM selectedMenu)
        {
            Order order = new Order();
            order.Quantity = selectedMenu.Quantity;
            order.MenuSize = selectedMenu.MenuSizes;
            order.UpdateDate = DateTime.Now;
            order.CreateDate = DateTime.Now;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            order.AppUserId = userId;
            order.MenuId = _db.Menus.Where(x => x.MenuName == selectedMenu.MenuName).FirstOrDefault().Id;
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            foreach (var item in selectedMenu.SauceSelections)
            {
                if (item.Quantity != 0)
                {
                    SauceOrder sauceOrder = new();
                    sauceOrder.Quantity = item.Quantity;
                    sauceOrder.SauceId = item.SauceId;
                    sauceOrder.OrderId = _db.Orders.Where(x => x.AppUserId == userId && x.IsItCompleted == false).OrderBy(x => x.Id).LastOrDefault().Id;
                    await _db.SauceOrders.AddAsync(sauceOrder);
                    await _db.SaveChangesAsync();
                }
            }

            return RedirectToAction("FiyatKaydet");
        }

        public async Task<IActionResult> OldOrders()
        {
            ListingAllBox();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var oldOrders = await _db.Orders.Where(x => x.AppUserId == userId && x.IsItCompleted == true).ToListAsync(); // burayı düzelt
            var orderIds = oldOrders.Select(o => o.Id).ToList();
            var orderPrices = await _db.OrderPrices.Where(x => orderIds.Contains(x.OrderId)).ToListAsync();
            var menus = await _db.Menus.ToListAsync();
            var orderSauces = await _db.SauceOrders.Where(x => orderIds.Contains(x.OrderId)).ToListAsync();
            var sauces = await _db.Sauces.ToListAsync();
            List<OldOrdersVM> oldOrdersVM = oldOrders.Select(x => new OldOrdersVM() { OrderId = x.Id, Name = menus.Where(x1 => x1.Id == x.MenuId).FirstOrDefault().MenuName, PicturePath = menus.Where(x1 => x1.Id == x.MenuId).FirstOrDefault().PicturePath, Price = orderPrices.Where(x2 => x2.OrderId == x.Id).FirstOrDefault().Price, Quantity = x.Quantity, MenuSize = x.MenuSize }).ToList();

            return View(oldOrdersVM);  
        }

        public void ListingAllBox()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fromOrders = _db.Orders.Where(x => x.AppUserId == userId && x.IsItCompleted == false).ToList();
            List<UserBoxVM> userOrders2 = fromOrders.Join(_db.Menus, x1 => x1.MenuId, x2 => x2.Id, (x1, x2) => new { OrderDb = x1, MenuDb = x2 }).Join(_db.OrderPrices, x3 => x3.OrderDb.Id, x4 => x4.OrderId, (x3, x4) => new { OrderMenuDb = x3.MenuDb, OrderOrderDb = x3.OrderDb, OrderPriceDb = x4 }).Select(x => new UserBoxVM() { Price = x.OrderPriceDb.Price, OrderName = x.OrderMenuDb.MenuName, OrderId = x.OrderOrderDb.Id }).ToList();
            ViewData["UserOrders"] = userOrders2;
        }

        public IActionResult MenuDetails(int OrderId)
        {
            ListingAllBox();

            ViewData["OrderId"] = OrderId;
            var targetOrder = _db.Orders.Where(x => x.Id == OrderId).FirstOrDefault();
            var targetSauces = _db.Sauces.ToList();
            var targetMenu = _db.Menus.Where(x => x.Id == targetOrder.MenuId).FirstOrDefault();
            var targetOrderPrice = _db.OrderPrices.Where(x => x.OrderId == targetOrder.Id).FirstOrDefault();
            var targetSauceOrders = _db.SauceOrders.Where(x => x.OrderId == targetOrder.Id).ToList();
            SelectedMenuVM selectedMenu = new() { MenuName = targetMenu.MenuName, Price = targetMenu.Price, PicturePath= targetMenu.PicturePath };
            selectedMenu.Sauces = targetSauces;
            selectedMenu.Quantity = targetOrder.Quantity;
            selectedMenu.MenuSizes = targetOrder.MenuSize;
            ViewData["LastPrice"] = targetOrderPrice.Price;
            foreach (var sauce in targetSauces)
            {
                var dd = targetSauceOrders.Where(x => x.SauceId == sauce.Id).FirstOrDefault();
                if (dd != null)
                {
                    selectedMenu.SauceSelections.Add(new SauceSelectionVM { SauceId = sauce.Id/*, IsSelected = false*/, Price = sauce.Price, Quantity = dd.Quantity });
                }
                else
                {
                    selectedMenu.SauceSelections.Add(new SauceSelectionVM { SauceId = sauce.Id/*, IsSelected = false*/, Price = sauce.Price});
                }
            }
            return View(selectedMenu);
        }
        public async Task<IActionResult> DeleteMenu(int orderId)
        {
            var targetMenu = await _db.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();
            _db.Orders.Remove(targetMenu);
            await _db.SaveChangesAsync();
            return RedirectToAction("MenuList", "MainPage", new { area = "" });

        }
        public async Task<IActionResult> RemoveAllMenus()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var targetMenus = await _db.Orders.Where(x => x.AppUserId == userId && x.IsItCompleted == false).ToListAsync();
            if (targetMenus.Count != 0)
            {
                _db.Orders.RemoveRange(targetMenus);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("MenuList", "MainPage", new { area = "" });

        }

        public async Task<IActionResult> UpdateMenu(UpdateMenuVM updateMenuVM)
        {
            var targetOrder = _db.Orders.Where(x => x.Id == updateMenuVM.OrderId).FirstOrDefault();
            targetOrder.Quantity = updateMenuVM.Quantity;
            targetOrder.MenuSize = updateMenuVM.MenuSizes;
            targetOrder.IsItCompleted = false; // yeni
            targetOrder.UpdateDate = DateTime.Now;
            _db.Orders.Update(targetOrder);
            var oldSauceOrders = _db.SauceOrders.Where(x => x.OrderId == updateMenuVM.OrderId).ToList();
            _db.RemoveRange(oldSauceOrders);  
            List<SauceOrder> newSauceOrders = updateMenuVM.SauceSelections.Where(x=> x.Quantity != 0).Select(x => new SauceOrder() { SauceId = x.SauceId, Quantity = x.Quantity, OrderId = updateMenuVM.OrderId}).ToList();
            await _db.SauceOrders.AddRangeAsync(newSauceOrders);
            await _db.SaveChangesAsync();

            return RedirectToAction("FiyatKaydet2", new {orderId = updateMenuVM.OrderId});
        }

        public async Task<IActionResult> FiyatKaydet2(int orderId)
        {
            decimal toplamFiyat = 0; int katSayi = 0;
            List<SosFiyatVM> sosFiyatListesi = _db.Sauces.Select(x => new SosFiyatVM()
            {
                Id = x.Id,
                Price = x.Price
            }).ToList();

            List<MenuFiyatVM> menuFiyatListesi = _db.Menus.Select(x => new MenuFiyatVM()
            {
                Id = x.Id,
                Price = x.Price
            }).ToList();

            var kullaniciSiparisleri = await _db.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();

            toplamFiyat = kullaniciSiparisleri.Quantity * PriceForMenuSize(menuFiyatListesi.Where(x => x.Id == kullaniciSiparisleri.MenuId).FirstOrDefault().Price, kullaniciSiparisleri.MenuSize);
            List<SauceOrder> kullaniciSoslari = await _db.SauceOrders.Where(x => x.OrderId == kullaniciSiparisleri.Id).ToListAsync();
            foreach (var forSauce in kullaniciSoslari)
            {
                toplamFiyat += forSauce.Quantity * kullaniciSiparisleri.Quantity * sosFiyatListesi.Where(x => x.Id == forSauce.SauceId).FirstOrDefault().Price;
            }
            OrderPrice orderPrice1 = await _db.OrderPrices.Where(x => x.OrderId == kullaniciSiparisleri.Id).FirstOrDefaultAsync();
            orderPrice1.Price = toplamFiyat;
             _db.OrderPrices.Update(orderPrice1);
            await _db.SaveChangesAsync();
            return RedirectToAction("MenuList", "MainPage", new { area = "" });

        }


        public async Task<IActionResult> FiyatKaydet()
        {
            decimal toplamFiyat = 0; int katSayi = 0;
            List<SosFiyatVM> sosFiyatListesi = _db.Sauces.Select(x => new SosFiyatVM()
            {
                Id = x.Id,
                Price = x.Price
            }).ToList();

            List<MenuFiyatVM> menuFiyatListesi = _db.Menus.Select(x => new MenuFiyatVM()
            {
                Id = x.Id,
                Price = x.Price
            }).ToList();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var kullaniciSiparisleri = await _db.Orders.Where(x => x.AppUserId == userId && x.IsItCompleted == false).OrderBy(x => x.Id).LastOrDefaultAsync();

            toplamFiyat = kullaniciSiparisleri.Quantity * PriceForMenuSize(menuFiyatListesi.Where(x => x.Id == kullaniciSiparisleri.MenuId).FirstOrDefault().Price, kullaniciSiparisleri.MenuSize);
            List<SauceOrder> kullaniciSoslari = await _db.SauceOrders.Where(x => x.OrderId == kullaniciSiparisleri.Id).ToListAsync();
            foreach (var forSauce in kullaniciSoslari)
            {
                toplamFiyat += forSauce.Quantity * kullaniciSiparisleri.Quantity * sosFiyatListesi.Where(x => x.Id == forSauce.SauceId).FirstOrDefault().Price;
            }
            OrderPrice orderPrice = new() { OrderId = kullaniciSiparisleri.Id, Price = toplamFiyat };
            await _db.OrderPrices.AddAsync(orderPrice);
            await _db.SaveChangesAsync();
            //}
            return RedirectToAction("MenuList", "MainPage", new { area = "" });

        }


        public decimal PriceForMenuSize(decimal price, MenuSize menuSize)
        {
            if (menuSize == MenuSize.Small)
            {
                price *= 1;
            }
            else if (menuSize == MenuSize.Medium)
            {
                price *= 1.2m;
            }
            else
                price *= 1.5m;
            return price;
        }

    }
}
