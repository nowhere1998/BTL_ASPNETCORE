using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTL_ASPNETCORE.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;

namespace BTL_ASPNETCORE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DishesController : Controller
    {
        private readonly RESTINAContext _context;

        public DishesController(RESTINAContext context)
        {
            _context = context;
        }

        // GET: Admin/Dishes
        public async Task<IActionResult> Index(int page = 1, string name = "")
        {
            int pageSize = 4;
            page = page < 1 ? 1 : page;
            var dishes = _context.Dishes.Include(x=>x.Category)
                .OrderByDescending(x => x.DishId)
                .Where(x => x.DishName.ToLower().Contains(name.ToLower()))
                .ToPagedList(page, pageSize);
            ViewData["name"] = name;
            return View(dishes);
        }

        // GET: Admin/Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var dish = await _context.Dishes
            //    .Include(d => d.Category)
            //    .FirstOrDefaultAsync(m => m.DishId == id);
            //if (dish == null)
            //{
            //    return NotFound();
            //}

            return Redirect("/home/detail?id="+id);
        }

        // GET: Admin/Dishes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/Dishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishId,DishName,Price,SalePrice,Size,Image,Description,Status,CategoryId")] Dish dish, IFormFile Image, string description="")
        {
            if (dish.SalePrice == null)
            {
                dish.SalePrice = 0;
            }
            if (dish.SalePrice > dish.Price)
            {
                ViewBag.ErrorSalePrice = "Sale price can not bigger than price!"; 
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", dish.CategoryId);
                return View(dish);
            }
            if(Image!=null && Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin/assets/images", Image.FileName);
                using(var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                dish.Image = "/admin/assets/images/"+ Image.FileName;
            }
            dish.Description = description;
            if (ModelState.IsValid)
            {
                _context.Add(dish);
                await _context.SaveChangesAsync();
                TempData["success"] = "Create new dish success!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        // GET: Admin/Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        // POST: Admin/Dishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DishId,DishName,Price,SalePrice,Size,Image,Description,Status,CategoryId")] Dish dish, IFormFile? Image, string oldImage)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }
            dish.Image = oldImage;
            if (Image != null && Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin/assets/images", Image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                dish.Image = "/admin/assets/images/" + Image.FileName;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Update dish success!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        // GET: Admin/Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.DishId == id);
            if (_context.OrderDetails.Any(x => x.DishId == dish.DishId))
            {
                TempData["danger"] = "Can not delete because has item related!";
                return RedirectToAction(nameof(Index));
            }
            if (dish == null)
            {
                return NotFound();
            }
            else
            {
                _context.Dishes.Remove(dish);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Delete dish success!";
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.DishId == id);
        }
    }
}
