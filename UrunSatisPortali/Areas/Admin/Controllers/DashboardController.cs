using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // Eklendi
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Brand> _brandRepo;
        private readonly IRepository<Message> _messageRepo;
        private readonly IRepository<Order> _orderRepo; // Sipariş tablosu eklendi
        private readonly UserManager<IdentityUser> _userManager; // Müşteri sayısı için

        public DashboardController(
            IRepository<Product> productRepo,
            IRepository<Category> categoryRepo,
            IRepository<Brand> brandRepo,
            IRepository<Message> messageRepo,
            IRepository<Order> orderRepo,
            UserManager<IdentityUser> userManager)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
            _messageRepo = messageRepo;
            _orderRepo = orderRepo;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            // 1. ÜST KART İSTATİSTİKLERİ (GERÇEK VERİLER)
            var orders = _orderRepo.GetAll()?.ToList() ?? new List<Order>();

            ViewBag.TotalSales = orders.Sum(x => x.TotalPrice); // Sahte 12500 silindi, gerçek toplam geldi
            ViewBag.OrderCount = orders.Count; // Gerçek sipariş sayısı
            ViewBag.UserCount = _userManager.Users.Count(); // Gerçek kayıtlı müşteri sayısı
            ViewBag.ProductCount = _productRepo.GetAll()?.Count() ?? 0;

            // 2. MESAJLAR
            var allMessages = _messageRepo.GetAll()?.ToList() ?? new List<Message>();
            ViewBag.NewMessagesCount = allMessages.Count(x => !x.IsRead);
            ViewBag.RecentMessages = allMessages
                                        .OrderByDescending(x => x.CreatedDate)
                                        .Take(5)
                                        .ToList();

            // 3. GRAFİK VERİLERİ (Kategori Dağılımı)
            var categoryGroup = _productRepo.GetAll("Category")?
                .GroupBy(p => p.Category?.Name ?? "Kategorisiz")
                .Select(g => new { Isim = g.Key, Adet = g.Count() })
                .ToList();

            if (categoryGroup != null && categoryGroup.Any())
            {
                ViewBag.CategoryLabels = categoryGroup.Select(x => x.Isim).ToArray();
                ViewBag.CategoryCounts = categoryGroup.Select(x => x.Adet).ToArray();
            }

            // 4. ANALİZLER (En Çok Satan 5 Ürün)
            // Yorum sayısına göre değil, SalesCount alanına göre sıralıyoruz
            ViewBag.TopProducts = _productRepo.GetAll()?
                .OrderByDescending(p => p.SalesCount)
                .Take(5)
                .Select(p => new {
                    Name = p.Name,
                    SalesCount = p.SalesCount, // Kendi modelindeki alan
                    Stock = p.Stock,
                    Price = p.Price,
                    Image = p.Image
                }).ToList();

            // 5. SON EKLENEN ÜRÜNLER (Model olarak döner)
            var lastProducts = _productRepo.GetAll("Category")?
                                 .OrderByDescending(p => p.CreatedDate)
                                 .Take(5)
                                 .ToList() ?? new List<Product>();

            return View(lastProducts);
        }
    }
}