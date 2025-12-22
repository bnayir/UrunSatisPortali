using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

[Authorize]
public class OrderController : Controller
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Product> _productRepo; // Ürün stoğu için eklendi
    private readonly UserManager<IdentityUser> _userManager;

    public OrderController(
        IRepository<Order> orderRepo,
        IRepository<Product> productRepo, // Constructor'a eklendi
        UserManager<IdentityUser> userManager)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _userManager = userManager;
    }

    // Kullanıcının geçmiş siparişlerini listeler
    public IActionResult Index()
    {
        var userId = _userManager.GetUserId(User);
        // Sadece giriş yapan kullanıcıya ait siparişleri çekiyoruz
        var orders = _orderRepo.GetAll()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.OrderDate)
            .ToList();

        return View(orders);
    }

    // Sipariş tamamlama sayfası (Checkout)
    [HttpGet]
    public IActionResult Checkout()
    {
        var cartJson = HttpContext.Session.GetString("CartSession");
        if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Index", "Home");

        var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        // Checkout sayfasındaki "Özet" kısmında toplam fiyatın görünmesi için
        ViewBag.TotalPrice = cartItems.Sum(x => x.Price * x.Quantity);

        return View(new Order());
    }

    // Siparişi onaylama ve veritabanına kayıt işlemi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Checkout(Order order)
    {
        var userId = _userManager.GetUserId(User);
        var cartJson = HttpContext.Session.GetString("CartSession");

        if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Index", "Home");

        // 1. Sepet verilerini listeye çevir
        var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        // 2. Siparişin temel bilgilerini ata
        order.UserId = userId;
        order.OrderDate = DateTime.Now;
        order.TotalPrice = cartItems.Sum(x => x.Price * x.Quantity);

        // 3. STOK GÜNCELLEME İŞLEMİ
        foreach (var item in cartItems)
        {
            // Veritabanından ürünün güncel halini bul
            var product = _productRepo.GetById(item.ProductId);
            if (product != null)
            {
                // Stoğu sipariş miktarı kadar azalt
                product.Stock -= item.Quantity;

                // Stok eksiye düşmesin kontrolü
                if (product.Stock < 0) product.Stock = 0;

                // Ürünü güncelle (Veritabanına yansıtır)
                _productRepo.Update(product);
            }
        }

        // 4. Siparişi Kaydet
        _orderRepo.Add(order);

        // 5. SEPETİ TEMİZLE
        HttpContext.Session.Remove("CartSession");
        HttpContext.Session.Remove("CartCount");

        // Kullanıcıya sipariş numarasını gönderen başarı sayfasına yönlendir
        return View("OrderSuccess", order.Id);
    }
}