using HamburgerOrderWebSite.Context;
using HamburgerOrderWebSite.Models;
using HamburgerOrderWebSite.ViewModels.ForEditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HamburgerOrderWebSite.Areas.Editor.Controllers
{
    [Authorize(Roles ="Editör, Yönetici")]
    [Area("Editor")]
    public class MenuController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public MenuController(ApplicationContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _WebHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MenuCreate()
        {
            return View();
        }
        public IActionResult SauceList()// yeni
        {
            return View(_db.Sauces.ToList());
        }
        public IActionResult SauceCreate() // yeni
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SauceCreate(CreateSauceVM createSauceVM)// yeni
        {
            var oldSauce = await _db.Sauces.ToListAsync();

            if(oldSauce.Any(sauce => sauce.SauceName.ToLower() == createSauceVM.SauceName.ToLower()))
            {
                ModelState.AddModelError("", "Bu sos zaten sistemde mevcut");
            }
            else
            {
                Sauce sauce = new Sauce() { SauceName = createSauceVM.SauceName, Price = createSauceVM.Price };
                await _db.Sauces.AddAsync(sauce);
                await _db.SaveChangesAsync();
                return RedirectToAction("SauceList", "Menu", new { area = "Editor" });
            }
            
            return View();
        }
        public async Task<IActionResult> SauceUpdate(int id)// yeni
        {
            return View(await _db.Sauces.Where(x => x.Id == id).FirstOrDefaultAsync());
        }
        [HttpPost]
        public async Task<IActionResult> SauceUpdate(Sauce sauce)// yeni
        {
            var targetSauce = await _db.Sauces.Where(x => x.SauceName == sauce.SauceName).FirstOrDefaultAsync();
            targetSauce.Price = sauce.Price;
            _db.Sauces.Update(targetSauce);
            await _db.SaveChangesAsync();
            return RedirectToAction("SauceList", "Menu", new { area = "Editor" });
        }
        public async Task<IActionResult> SauceDelete(int id)// yeni
        {
            _db.Sauces.Remove(await _db.Sauces.Where(x => x.Id == id).FirstOrDefaultAsync());
            await _db.SaveChangesAsync();
            return RedirectToAction("SauceList", "Menu", new { area = "Editor" });
        }
        public async Task<IActionResult> MenuSil(string selectedMenuName)
        {
            var targetMenu = _db.Menus.Where(x => x.MenuName == selectedMenuName).FirstOrDefault();
            _db.Menus.Remove(targetMenu);
            await _db.SaveChangesAsync();
            return RedirectToAction("MenuList", "MainPage", new { area = "" });
        }
        public IActionResult MenuGuncelle(string selectedMenuName)
        {
            var targetMenu = _db.Menus.Where(x => x.MenuName == selectedMenuName).FirstOrDefault();
            MenuUpdateVM menuUpdateVM = new MenuUpdateVM() { MenuName = targetMenu.MenuName, PicturePath = targetMenu.PicturePath, Price = targetMenu.Price };
            return View(menuUpdateVM);
        }
        [HttpPost]
        public async Task<IActionResult> MenuGuncelle(MenuUpdateVM menuUpdateVM)
        {
            var targetMenu = _db.Menus.Where(x => x.MenuName == menuUpdateVM.MenuName).FirstOrDefault();
            string stringFileName = null;
            if (menuUpdateVM.NewPicturePath != null)
                stringFileName = UploadFile(menuUpdateVM);
            else if (targetMenu != null)
                stringFileName = targetMenu.PicturePath;
            else
                stringFileName = "ForEmpty.png";


            if (targetMenu == null)
            {
                var createMenu = new Menu { MenuName = menuUpdateVM.MenuName, Price = menuUpdateVM.Price, PicturePath = stringFileName };
                await _db.Menus.AddAsync(createMenu);
            }
            else
            {
                targetMenu.PicturePath = stringFileName;
                targetMenu.MenuName = menuUpdateVM.MenuName;
                targetMenu.Price = menuUpdateVM.Price;
                _db.Menus.Update(targetMenu);
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("MenuList", "MainPage", new { area = "" });
        }



        private string UploadFile(MenuUpdateVM menuUpdateVM)
        {
            string fileName = null;
            if (menuUpdateVM != null)
            {
                string uploadDir = Path.Combine(_WebHostEnvironment.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "-" + menuUpdateVM.NewPicturePath.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    menuUpdateVM.NewPicturePath.CopyTo(fileStream);
                }
            }
            return fileName;
        }



    }
}
