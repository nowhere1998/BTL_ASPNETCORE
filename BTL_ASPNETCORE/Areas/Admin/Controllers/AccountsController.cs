using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTL_ASPNETCORE.Models;
using X.PagedList.Extensions;

namespace BTL_ASPNETCORE.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly RESTINAContext _context;

        public AccountsController(RESTINAContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index(int page = 1, string name = "")
        {
            int pageSize = 4;
            page = page < 1 ? 1 : page;
            var accounts = _context.Accounts.OrderByDescending(x => x.AccountId)
                .Where(x => x.Name.ToLower().Contains(name.ToLower()))
                .ToPagedList(page, pageSize);
            ViewData["name"] = name;
            return View(accounts);
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Name,Password,Email,Phone,Role,Image")] Account account, IFormFile Image, string confirmPassword="")
        {
            if (_context.Accounts.Any(x => x.Email == account.Email))
            {
                ViewBag.Email = "Email is already exist!";
                return View(account);
            }
            if (confirmPassword.Equals(account.Password))
            {
                account.Password = Cipher.GenerateMD5(account.Password);
            }
            else
            {
                ViewBag.ErrorConfirmPassword = "Confirm password does not match!";
                return View(account);
            }
            if (Image != null && Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin/assets/images", Image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                account.Image = "/admin/assets/images/" + Image.FileName;
            }
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                TempData["success"] = "Create new account success!";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Name,Password,Email,Phone,Role,Image")] Account account, IFormFile? Image, string oldImage="", string confirmPassword="")
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }
            if (!account.Password.Equals(confirmPassword))
            {
                ViewBag.ErrorConfirmPassword = "Confirm password does not match!";
                return View(account);
            }
            if (HttpContext.User.Claims.Skip(1).FirstOrDefault().Value.Equals(account.AccountId.ToString()))
            {
                account.Password = Cipher.GenerateMD5(account.Password);
            }
            
            account.Image = oldImage;
            if (Image != null && Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin/assets/images", Image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                account.Image = "/admin/assets/images/" + Image.FileName;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Update account success!";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
            account.Role = 2;
            _context.Update(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Admin/Accounts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var account = await _context.Accounts.FindAsync(id);
        //    if (account != null)
        //    {
        //        _context.Accounts.Remove(account);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
