using BTL_ASPNETCORE.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BTL_ASPNETCORE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    public class AdminHomeController : Controller
    {
        private readonly RESTINAContext _context;
        public AdminHomeController(RESTINAContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("search")]
        public IActionResult Search(string search, string path)
        {
            if (path.ToLower().Contains("order/details"))
            {
                return Redirect("/admin/orders" + "?name=" + search);
            }
            return Redirect(path+"?name="+search);
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string email, string password)
        {
            string passMD5 = "";
            if (password.IsNullOrEmpty())
            {
                password = "";
            }
            passMD5 = Cipher.GenerateMD5(password);
            var acc = _context.Accounts.FirstOrDefault(x => x.Email == email && x.Password == passMD5 && x.Role == 1);
            if (acc != null)
            {
                var identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, acc.Name??""),
                        new Claim("accountId", acc.AccountId.ToString()),
                        new Claim("image", acc.Image??""),
                        new Claim("email", acc.Email??"")
                    }, "RESTINASecurityScheme");
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync("RESTINASecurityScheme", principal);
                return RedirectToAction("Index");
            }
            ViewBag.error = "<p class='alert alert-danger'>Email or password is incorrect!</p>";
            ViewBag.Email = email;
            ViewBag.Password = password;    
            return View();
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("RESTINASecurityScheme");
            return Redirect("~/admin");
        }
    }
}
