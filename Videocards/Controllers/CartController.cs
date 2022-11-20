using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Videocards.Models;

namespace Videocards.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private VCContext db;

        public CartController(VCContext context)
        {
            db = context;
        }

        public IActionResult Add(int id)
        {
            string cartId;
            if (HttpContext.Request.Cookies.Keys.Count > 0 &&
                HttpContext.Request.Cookies.Keys.Contains("CartId"))
            {
                cartId = HttpContext.Request.Cookies["CartId"];
            }
            else
            {
                cartId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append("CartId", cartId);
            }
            var query = db.ShoppingCarts.Where(c => c.CartId == cartId && c.VCid == id);
            if (query.Any())
            {
                CartItem cart = query.First();
                var list = db.VideoCards.Where(v => v.Id == id);
                var vc = list.FirstOrDefault();
                cart.SelectVC = vc;
                db.Entry(cart).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else // такой видеокарты в корзине нет
            {
                var item = new CartItem()
                {
                    VCid = id,
                    CartId = cartId
                };
                db.ShoppingCarts.Add(item);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Delete(int id)
        {
            var cartItem = db.ShoppingCarts.Find(id);
            db.ShoppingCarts.Remove(cartItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            string cartId = null;
            if (HttpContext.Request.Cookies.Keys.Count > 0 &&
                HttpContext.Request.Cookies.Keys.Contains("CartId"))
            {
                cartId = HttpContext.Request.Cookies["CartId"];
            }
            List<CartItem> cartList = new List<CartItem>();
            List<int> costList = new List<int>();
            if (cartId != null)
            {
                cartList = db.ShoppingCarts.Where(c => c.CartId == cartId).ToList();
                int sum = 0;
                foreach (var item in cartList)
                {
                    var vc = db.VideoCards.Find(item.VCid);
                    item.SelectVC = vc;
                    int cost = vc.Price;
                    sum += cost;
                    costList.Add(cost);
                }
                ViewBag.Sum = sum;
                ViewBag.Cost = costList;
            }
            ViewBag.Msg = cartList.Count == 0 ?
                "Ваша корзина пуста. Надо туда что-то положить" : "Ваши заказ:";
            return View(cartList);
        }
    }
}
