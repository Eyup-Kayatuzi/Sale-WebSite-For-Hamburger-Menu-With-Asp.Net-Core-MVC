using HamburgerOrderWebSite.Context;
using HamburgerOrderWebSite.Models;
using HamburgerOrderWebSite.ViewModels;
using HamburgerOrderWebSite.ViewModels.ForAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace HamburgerOrderWebSite.Areas.Admin.Controllers
{
    [Authorize(Roles = "Yönetici")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationContext _db;

        public AdminController(ApplicationContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> OrderInfo()
        {
            ForOrderInfoVM forOrderInfoVM = new ForOrderInfoVM();
            var menuNames = await _db.Menus.ToListAsync();
            var sauceNames = await _db.Sauces.ToListAsync();


            foreach (var menu in menuNames) 
            {
                forOrderInfoVM.Menus.Add(menu.MenuName);
                forOrderInfoVM.Prices.Add(_db.Orders.Where(x => x.IsItCompleted == true && x.MenuId == menu.Id).Join(_db.OrderPrices, x1 => x1.Id, x2 => x2.OrderId, (x1, x2) => x2.Price).Sum()); // false to true
            }
            foreach (var sauce in sauceNames)
            {
                forOrderInfoVM.SauceNames.Add(sauce.SauceName);
                forOrderInfoVM.SauceQuantity.Add(_db.Orders.Where(x => x.IsItCompleted == true).Join(_db.SauceOrders.Where(x => x.SauceId == sauce.Id), x1 => x1.Id, x2 => x2.OrderId, (x1, x2) => new { OrderDb = x1, SauceOrderDb = x2 }).Sum(x => x.OrderDb.Quantity * x.SauceOrderDb.Quantity)* sauce.Price); // false to true
            }
            return View(forOrderInfoVM);
        }

        public async Task<IActionResult> AddNewEditor()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewEditor(SignUpVM signUpVM)
        {
            AppUser appUser = new AppUser()
            {
                UserName = signUpVM.UserName,
                Email = signUpVM.Email,
                CreateDate = DateTime.Now,
                Address = "",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var checkForEmail = await _userManager.FindByEmailAsync(appUser.Email);
            if (checkForEmail == null)
            {
                IdentityResult result = await _userManager.CreateAsync(appUser, signUpVM.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "EDITOR");
                    appUser.EmailConfirmed = true;
                    await _userManager.UpdateAsync(appUser);
                    TempData["EditorEkleme"] = "Hesap oluşturulurken bir hata oluştu";
                    return RedirectToAction("MenuList", "MainPage", new { area = "" });
                }
                else
                {
                    ViewData["EditorEkleme"] = "Hesap oluşturulurken bir hata oluştu";
                    return View();
                }

            }
            else
            {
                ViewData["EditorEkleme"] = "Bu mail adresi sistemde mevcuttur";
                return View();
            }
            
        }
    }
}
