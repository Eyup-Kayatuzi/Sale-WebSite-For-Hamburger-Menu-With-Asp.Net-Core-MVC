using HamburgerOrderWebSite.Context;
using HamburgerOrderWebSite.Enums;
using HamburgerOrderWebSite.Models;
using HamburgerOrderWebSite.ViewModels;
using HamburgerOrderWebSite.ViewModels.ForMenu;
using HamburgerOrderWebSite.ViewModels.ForUser;
using MailMessageService.Models;
using MailMessageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HamburgerOrderWebSite.Controllers
{
    public class MainPageController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationContext _db;
        private readonly IEmailService _emailService;


        public MainPageController(ApplicationContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _emailService = emailService;
        }
        public IActionResult MenuList()
        {
            ListingAllBox();

            var menus = _db.Menus.ToList();
            return View(menus);
        }

        private void ListingAllBox()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fromOrders = _db.Orders.Where(x => x.AppUserId == userId && x.IsItCompleted == false).ToList();
            List<UserBoxVM> userOrders2 = fromOrders.Join(_db.Menus, x1 => x1.MenuId, x2 => x2.Id, (x1, x2) => new { OrderDb = x1, MenuDb = x2 }).Join(_db.OrderPrices, x3 => x3.OrderDb.Id, x4 => x4.OrderId, (x3, x4) => new { OrderMenuDb = x3.MenuDb, OrderOrderDb = x3.OrderDb, OrderPriceDb = x4 }).Select(x => new UserBoxVM() { Price = x.OrderPriceDb.Price, OrderName = x.OrderMenuDb.MenuName, OrderId = x.OrderOrderDb.Id }).ToList();
            ViewData["UserOrders"] = userOrders2;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModelVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);


                if (result.Succeeded)
                {
                    return RedirectToAction("MenuList");
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınızı etkinleştirmeniz gerekmektedir. Lütfen e-posta adresinizi kontrol edin ve onaylayın.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Kullanıcı adınızda veya şifrenizde hata var. Lütfen tekrar deneyin.");
                    }
                    return View(model);
                }
            }
        }

        public async Task<IActionResult> SignUp()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpVM userVM, string role = "USER")
        {
            ModelState.Remove("role");
            if (ModelState.IsValid == true)
            {
                AppUser appUser = new AppUser()
                {
                    UserName = userVM.UserName,
                    Email = userVM.Email,
                    Address = userVM.Address,
                    PhoneNumber = userVM.PhoneNumber,
                    CreateDate = DateTime.Now,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var checkForEmail = await _userManager.FindByEmailAsync(appUser.Email);
                
                if (checkForEmail == null)
                {
                    IdentityResult result = await _userManager.CreateAsync(appUser, userVM.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(appUser, role);

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                        var emailDogrulamaLinki = Url.Action(nameof(ConfirmEmail), "MainPage", new { token, email = appUser.Email }, protocol: "https");

                        var emailDogrulamaMesaji = new MailMessage(new Dictionary<string, string>() { { "Mail Adresi Doğrulama"!, appUser.Email! } }, "Email Doğrulama Linki", $"<b>Uygulamamıza giriş yapabilmeniz için doğrulama linki:</b> {emailDogrulamaLinki!}");

                        _emailService.SendEmail(emailDogrulamaMesaji);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            if (error.Code == "DuplicateUserName")
                                ModelState.AddModelError("", "Bu kullanıcı adı sistemde mevcut");
                            else
                                ModelState.AddModelError("", error.Description);
                        }
                        return View(userVM);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bu mail adresi sistemde mevcuttur");
                    return View(userVM);
                }
               
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error.ErrorMessage);
                    }
                }
                return View(userVM);
            }


        }

        public async Task<ActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok("Kullanıcı maili onaylandı");
                }
                else
                {
                    return Ok("Onaylanma Başarısız");
                }
            }
            else
            {
                return Ok("Kullanıcı Sistemde bulunmamaktadır");
            }
        }



    }
}
