using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using UrunSatisPortali.Hubs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UrunSatisPortali.Controllers
{
    public class CartController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IHubContext<DashboardHub> _hubContext;
        private const string CartSessionKey = "CartSession";
        private const string CartCountKey = "CartCount";

        public CartController(IRepository<Product> productRepo, IRepository<Order> orderRepo, IHubContext<DashboardHub> hubContext)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _hubContext = hubContext;
        }

        // --- YARDIMCI METOTLAR (SESSION İŞLEMLERİ) ---
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

        // --- SEPETİ GÖRÜNTÜLEME ---
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            ViewBag.TotalPrice = cart.Sum(x => x.Price * x.Quantity);
            return View(cart);
        }

        // --- SEPETE ÜRÜN EKLEME ---
        public IActionResult AddToCart(int id)
        {
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
                    cart.Add(new CartItem
                    {
                        ProductId = id,
                        Product = product,
                        Price = product.Price,
                        Quantity = 1
                    });
                }

                SaveCartToSession(cart);
            }

            return RedirectToAction("Index", "Home");
        }

        // --- SEPETTEN ÜRÜN SİLME ---
        public IActionResult Remove(int id)
        {
            var cart = GetCartFromSession();
            cart.RemoveAll(x => x.ProductId == id);
            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }

        // --- ADET AZALTMA ---
        public IActionResult Decrease(int id)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
                else
                    cart.Remove(item);

                SaveCartToSession(cart);
            }
            return RedirectToAction("Index");
        }

        // --- KRİTİK EKLENTİ: SATIN ALMA VE SIGNALR TETİKLEME ---
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCartFromSession();
            if (!cart.Any()) return RedirectToAction("Index");

            // 1. Yeni sipariş oluştur ve kaydet
            var order = new Order
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                OrderDate = DateTime.Now,
                TotalPrice = cart.Sum(x => x.Price * x.Quantity),
                Status = "Sipariş Alındı",
                City = "Belirtilmedi",
                FirstName = User.Identity.Name ?? "Misafir",
                LastName = "",
                Address = "Sistemden Otomatik Onay",
                Phone = "000"
            };

            _orderRepo.Add(order); // Veritabanına fiziksel kayıt

            // 2. Sepeti temizle
            HttpContext.Session.Remove(CartSessionKey);
            HttpContext.Session.Remove(CartCountKey);

            // 3. SIGNALR TETİKLEMESİ: Admin Paneline Anlık Haber Ver
            var currentOrderCount = _orderRepo.GetAll().Count();
            var currentTotalSales = _orderRepo.GetAll().Sum(x => x.TotalPrice).ToString("C2");

            // Admin sayfasındaki JavaScript "ReceiveOrderUpdate" fonksiyonunu tetikler
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", currentOrderCount, currentTotalSales);

            return RedirectToAction("Index", "Order");
        }
    }
}