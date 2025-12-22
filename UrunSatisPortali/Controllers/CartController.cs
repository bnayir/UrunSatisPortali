using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json; // JSON serileştirme için gerekli

namespace UrunSatisPortali.Controllers
{
    public class CartController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private const string CartSessionKey = "CartSession"; // Session anahtarı
        private const string CartCountKey = "CartCount";     // Navbar sayacı için anahtar

        public CartController(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        // Sepet listesini getiren yardımcı metot
        private List<CartItem> GetCartFromSession()
        {
            var jsonStr = HttpContext.Session.GetString(CartSessionKey);
            return jsonStr == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(jsonStr);
        }

        // Sepeti kaydeden ve sayacı güncelleyen yardımcı metot
        private void SaveCartToSession(List<CartItem> cart)
        {
            var jsonStr = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, jsonStr);

            // NAVBAR SAYACINI GÜNCELLE
            int totalCount = cart.Sum(x => x.Quantity);
            HttpContext.Session.SetString(CartCountKey, totalCount.ToString());
        }

        public IActionResult Index()
        {
            return View(GetCartFromSession());
        }

        public IActionResult AddToCart(int id)
        {
            var product = _productRepo.GetById(id);
            if (product != null)
            {
                var cart = GetCartFromSession();
                var item = cart.FirstOrDefault(x => x.ProductId == id);

                if (item != null) item.Quantity++;
                else cart.Add(new CartItem { ProductId = id, ProductName = product.Name, Price = product.Price, Quantity = 1 });

                SaveCartToSession(cart);
            }

            // Sepete ekledikten sonra ana sayfaya dön (Kullanıcı alışverişe devam etsin)
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCartFromSession();
            cart.RemoveAll(x => x.ProductId == id);
            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }
    }
}