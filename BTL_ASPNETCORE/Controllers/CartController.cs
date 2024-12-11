using BTL_ASPNETCORE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BTL_ASPNETCORE.Controllers
{
    public class CartController : Controller
    {
        private readonly RESTINAContext _context;
        private List<Cart> carts = new List<Cart>();

        public CartController(RESTINAContext context)
        {
            _context = context;
        }

        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            return View(carts);
        }

        public IActionResult Add(int id, int quantity = 1)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            if (carts.Any(x=>x.Id == id))
            {
                carts.Where(x => x.Id == id).First().Quantity += quantity;
            }
            else
            {
                var test = id;
                var dish = _context.Dishes.Find(id);
                if(dish != null)
                {
                    var item = new Cart { Id = dish.DishId, Name = dish.DishName, Image=dish.Image, Price = dish.SalePrice<=0?dish.Price:dish.SalePrice, Quantity=quantity };
                    carts.Add(item);
                }
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(carts));

            return RedirectToAction("Cart", "Cart");
        }

        public IActionResult Update(int id, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            if (carts.Any(x=> x.Id == id))
            {
                carts.Where(x=>x.Id==id).First().Quantity = quantity;
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(carts));
            }
            return RedirectToAction("Cart", "Cart");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            if (carts.Any(x=> x.Id == id))
            {
                var item = carts.Where(x=> x.Id == id).First();
                carts.Remove(item);
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(carts));
            }
            return RedirectToAction("Cart", "Cart");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Menu", "Home");
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            return View(carts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(string name = "", string phone="", string address="", float totalPrice=0, int accountId=0)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            else
            {
                ViewBag.ErrorCheckout = "Please choose dishes!";
                return View(carts);
            }
            Order order = new Order();
            if (name.IsNullOrEmpty())
            {
                order.Name = HttpContext.Session.GetString("username");
            }
            else
            {
                order.Name = name;
            }
            order.Phone = phone;
            order.Address = address;
            order.TotalPrice = totalPrice;
            order.AccountId = accountId;
            if (ModelState.IsValid)
            {
                _context.Add(order);
                _context.SaveChanges();
                order = _context.Orders.OrderByDescending(x=>x.OrderId).First();
                foreach(var item in carts)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.DishId = item.Id;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.Price = item.Price;
                    _context.Add(orderDetail);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index", "Home", new {message = "Checkout success!" });
            }
            return View(carts);
        }
    }
}
