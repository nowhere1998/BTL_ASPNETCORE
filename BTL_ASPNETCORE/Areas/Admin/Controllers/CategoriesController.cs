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
    public class CategoriesController : Controller
    {
        private readonly RESTINAContext _context;

        public CategoriesController(RESTINAContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index(int page=1, string name="")
        {
            int pageSize = 4;
            page = page < 1 ? 1 : page;
            var categories = _context.Categories.OrderByDescending(x => x.CategoryId)
                .Where(x=>x.CategoryName.ToLower().Contains(name.ToLower()))
                .ToPagedList(page, pageSize);
            ViewData["name"] = name;
            return View(categories);
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Status")] Category category)
        {            
            if (ModelState.IsValid)
            {
                if (_context.Categories.Any(x => x.CategoryName.ToLower().Equals(category.CategoryName.ToLower())))
                {
                    ViewBag.ErrorName = "Name is already exist!";
                    return View(category);
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Create new category success!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Status")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Categories.Any(x => x.CategoryName.ToLower().Equals(category.CategoryName.ToLower()) && x.CategoryId!=category.CategoryId))
                    {
                        ViewBag.ErrorName = "Name is already exist!";
                        return View(category);
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Update category success!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (_context.Dishes.Any(x => x.CategoryId == category.CategoryId))
            {
                TempData["danger"] = "Can not delete because has item related!";
                return RedirectToAction(nameof(Index));
            }
            if (category == null)
            {
                return NotFound();
            }
            else
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Delete category success!";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
