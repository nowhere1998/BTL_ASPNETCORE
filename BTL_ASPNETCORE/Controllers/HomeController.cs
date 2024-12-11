using BTL_ASPNETCORE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using X.PagedList.Extensions;

namespace BTL_ASPNETCORE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RESTINAContext _context;

        public HomeController(ILogger<HomeController> logger, RESTINAContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string message)
        {
            if (message != null)
            {
                ViewBag.CheckoutSuccess = message;
            }
            ViewBag.Chef = _context.Chefs.OrderByDescending(c=>c.ChefId).Skip(0).Take(4).ToList();
            ViewBag.PopularDish = _context.Dishes
                .OrderByDescending(x=>x.DishId).Skip(0).Take(6).ToList();
            return View();
        }

        public IActionResult Login(string page="")
        {
            ViewBag.Page = page;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email="", string password="", string page = "")
        {
            string passmd5 = "";
            passmd5 = Cipher.GenerateMD5(password);
            var acc = _context.Accounts.SingleOrDefault(x => x.Email == email && x.Password == passmd5);
            if (acc != null && acc.Role != 2)
            {
                HttpContext.Session.SetString("username", acc.Name);
                HttpContext.Session.SetString("accountId", acc.AccountId.ToString());

                if (page.ToLower().Contains("checkout"))
                {
                    return RedirectToAction("Checkout", "Cart");
                }
                return RedirectToAction("Index");
            }
            ViewBag.error = "<p class='alert alert-danger'>Email or password is incorrect!</p>";
            ViewBag.Email = email;
            ViewBag.password = password;
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("Name, Email, Password")] Account account, string confirmPassword="")
        {
            if(_context.Accounts.Any(x=>x.Email == account.Email))
            {
                ViewBag.Email = "Email is already exist!";
                return View(account);
            }
            if(confirmPassword.Equals(account.Password))
            {
                account.Password = Cipher.GenerateMD5(account.Password);
            }
            else
            {
                ViewBag.ErrorConfirmPassword = "Confirm password does not match!";
                return View(account);
            }
            if(ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return Redirect("/home/login");
            }
            return View(account);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Menu(int page = 1, string name = "", string categoryId = "")
        {
            int pagesize = 6;
            page = page < 1 ? 1 : page;
            if(categoryId.IsNullOrEmpty())
            {
                var dishes = _context.Dishes.Include(x => x.Category)
                .OrderByDescending(x => x.DishId)
                .Where(x => x.DishName.ToLower().Contains(name.ToLower()))
                .ToPagedList(page, pagesize);
                ViewData["name"] = name;
                ViewData["categoryId"] = categoryId;
                ViewBag.Category = _context.Categories.ToList();
                return View(dishes);
            }
            else
            {
                var dishes = _context.Dishes.Include(x => x.Category)
                .OrderByDescending(x => x.DishId)
                .Where(x => x.DishName.ToLower().Contains(name.ToLower()) && x.CategoryId==int.Parse(categoryId))
                .ToPagedList(page, pagesize);
                ViewData["name"] = name;
                ViewData["categoryId"] = categoryId;
                ViewBag.Category = _context.Categories.ToList();
                return View(dishes);
            }
        }

        public IActionResult Detail(int id=0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var dish = _context.Dishes.Include(x=>x.Category).FirstOrDefault(x=>x.DishId==id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Branch()
        {
            return View();
        }

        public IActionResult Chef()
        {
            var chefs = _context.Chefs.OrderByDescending(c=>c.ChefId).ToList();
            return View(chefs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
