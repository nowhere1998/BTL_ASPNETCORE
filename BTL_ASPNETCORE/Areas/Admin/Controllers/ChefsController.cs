using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTL_ASPNETCORE.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;

namespace BTL_ASPNETCORE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ChefsController : Controller
    {
        private readonly RESTINAContext _context;

        public ChefsController(RESTINAContext context)
        {
            _context = context;
        }

        // GET: Admin/Chefs
        public async Task<IActionResult> Index(int page = 1, string name = "")
        {
            int pageSize = 4;
            page = page < 1 ? 1 : page;
            var chefs = _context.Chefs.OrderByDescending(x => x.ChefId)
                .Where(x => x.ChefName.ToLower().Contains(name.ToLower()))
                .ToPagedList(page, pageSize);
            ViewData["name"] = name;
            return View(chefs);
        }

        // GET: Admin/Chefs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chef = await _context.Chefs
                .FirstOrDefaultAsync(m => m.ChefId == id);
            if (chef == null)
            {
                return NotFound();
            }

            return View(chef);
        }

        // GET: Admin/Chefs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Chefs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChefId,ChefName,Image,Description")] Chef chef, IFormFile Image)
        {
            if (Image != null && Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin/assets/images", Image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                chef.Image = "/admin/assets/images/" + Image.FileName;
            }
            if (ModelState.IsValid)
            {
                _context.Add(chef);
                await _context.SaveChangesAsync();
                TempData["success"] = "Create new chef success!";
                return RedirectToAction(nameof(Index));
            }
            return View(chef);
        }

        // GET: Admin/Chefs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chef = await _context.Chefs.FindAsync(id);
            if (chef == null)
            {
                return NotFound();
            }
            return View(chef);
        }

        // POST: Admin/Chefs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChefId,ChefName,Image,Description")] Chef chef, IFormFile? Image, string oldImage)
        {
            if (id != chef.ChefId)
            {
                return NotFound();
            }
            chef.Image = oldImage;
            if (Image != null && Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin/assets/images", Image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                chef.Image = "/admin/assets/images/" + Image.FileName;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chef);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChefExists(chef.ChefId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Update chef success!";
                return RedirectToAction(nameof(Index));
            }
            return View(chef);
        }

        // GET: Admin/Chefs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chef = await _context.Chefs
                .FirstOrDefaultAsync(m => m.ChefId == id);
            if (chef == null)
            {
                return NotFound();
            }
            else
            {
                _context.Chefs.Remove(chef);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Delete chef success!";
            return RedirectToAction(nameof(Index));
        }

        private bool ChefExists(int id)
        {
            return _context.Chefs.Any(e => e.ChefId == id);
        }
    }
}
