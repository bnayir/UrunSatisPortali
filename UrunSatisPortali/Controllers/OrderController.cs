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
    private readonly IRepository<Product> _productRepo;
    private readonly UserManager<IdentityUser> _userManager;

    public OrderController(
        IRepository<Order> orderRepo,
        IRepository<Product> productRepo,
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

        // Sepet toplamını hesaplayıp sayfaya gönderiyoruz
        ViewBag.TotalPrice = cartItems.Sum(x => x.Price * x.Quantity);

        return View(new Order());
    }

    // Siparişi onaylama, stok düşürme ve satış sayısını artırma
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Checkout(Order order)
    {
        var userId = _userManager.GetUserId(User);
        var cartJson = HttpContext.Session.GetString("CartSession");

        if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Index", "Home");

        var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        // 1. Siparişin temel bilgilerini ata
        order.UserId = userId;
        order.OrderDate = DateTime.Now;
        order.TotalPrice = cartItems.Sum(x => x.Price * x.Quantity);

        // 2. STOK VE SATIŞ SAYACI GÜNCELLEME
        foreach (var item in cartItems)
        {
            var product = _productRepo.GetById(item.ProductId);
            if (product != null)
            {
                // Stoktan düşüyoruz
                product.Stock -= item.Quantity;
                if (product.Stock < 0) product.Stock = 0;

                // Satış sayacını artırıyoruz (Anasayfa için)
                product.SalesCount += item.Quantity;

                _productRepo.Update(product);
            }
        }

        // 3. Siparişi Kaydet
        _orderRepo.Add(order);

        // 4. SEPETİ TEMİZLE
        HttpContext.Session.Remove("CartSession");
        HttpContext.Session.Remove("CartCount");

        // Başarı sayfasına yönlendir
        return View("OrderSuccess", order.Id);
    }

    // Sipariş Detayı
    public IActionResult Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var order = _orderRepo.GetById(id);

        // Güvenlik: Kullanıcı sadece kendi sipariş detayını görmeli
        if (order == null || order.UserId != userId) return NotFound();

        return View(order);
    }
}