using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace UrunSatisPortali.Controllers
{
    public class CartController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private const string CartSessionKey = "CartSession";
        private const string CartCountKey = "CartCount";

        public CartController(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        private List<CartItem> GetCartFromSession()
        {
            var jsonStr = HttpContext.Session.GetString(CartSessionKey);
            return jsonStr == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(jsonStr);
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var jsonStr = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, jsonStr);

            int totalCount = cart.Sum(x => x.Quantity);
            HttpContext.Session.SetString(CartCountKey, totalCount.ToString());
        }

        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            // Sepet sayfasında toplam fiyatı göstermek için ViewBag kullanabilirsin
            ViewBag.TotalPrice = cart.Sum(x => x.Price * x.Quantity);
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            // Ürünü tüm ilişkileriyle (Marka vb.) çekmek daha iyidir
            var product = _productRepo.GetById(id);
            if (product != null)
            {
                var cart = GetCartFromSession();
                var item = cart.FirstOrDefault(x => x.ProductId == id);

                if (item != null)
                {
                    item.Quantity++;
                }
                else
                {
                    // DÜZELTİLEN KISIM: ProductName yerine Product nesnesini atıyoruz
                    cart.Add(new CartItem
                    {
                        ProductId = id,
                        Product = product, // Nesne atandığı için ProductName otomatik dolacak
                        Price = product.Price,
                        Quantity = 1
                    });
                }

                SaveCartToSession(cart);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCartFromSession();
            cart.RemoveAll(x => x.ProductId == id);
            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }

        // Adet artırma/azaltma için ek yardımcı metotlar (Şık durur)
        public IActionResult Decrease(int id)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                if (item.Quantity > 1) item.Quantity--;
                else cart.Remove(item);
                SaveCartToSession(cart);
            }
            return RedirectToAction("Index");
        }
    }
}