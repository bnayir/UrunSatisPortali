using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<Message> _messageRepo;

        public DashboardController(
            IRepository<Product> productRepo,
            IRepository<Category> categoryRepo,
            IRepository<Brand> brandRepo,
            IRepository<Comment> commentRepo,
            IRepository<Message> messageRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
            _commentRepo = commentRepo;
            _messageRepo = messageRepo;
        }

        public IActionResult Index()
        {
            // 1. ÜST KART İSTATİSTİKLERİ
            ViewBag.ProductCount = _productRepo.GetAll()?.Count() ?? 0;
            ViewBag.CategoryCount = _categoryRepo.GetAll()?.Count() ?? 0;
            ViewBag.BrandCount = _brandRepo.GetAll()?.Count() ?? 0;
            ViewBag.CommentCount = _commentRepo.GetAll()?.Count() ?? 0;

            // Dashboard tasarımı için gerekli simülasyon verileri
            ViewBag.TotalSales = 12500.75m;
            ViewBag.OrderCount = 24;
            ViewBag.UserCount = 8;

            // 2. MESAJLAR - GÜNCELLENMİŞ KISIM
            // Önce tüm mesajları listeye alıyoruz ki null hatası almayalım
            var allMessages = _messageRepo.GetAll()?.ToList() ?? new List<Message>();

            // Okunmamış mesaj sayısını IsRead durumuna göre filtreliyoruz
            ViewBag.NewMessagesCount = allMessages.Count(x => !x.IsRead);

            // Son 5 mesajı tarihe göre sıralayıp ViewBag'e gönderiyoruz
            ViewBag.RecentMessages = allMessages
                                        .OrderByDescending(x => x.CreatedDate)
                                        .Take(5)
                                        .ToList();

            // 3. GRAFİK VERİLERİ
            var categoryGroup = _productRepo.GetAll("Category")?
                .GroupBy(p => p.Category?.Name ?? "Kategorisiz")
                .Select(g => new { Isim = g.Key, Adet = g.Count() })
                .ToList();

            if (categoryGroup != null && categoryGroup.Any())
            {
                ViewBag.CategoryLabels = categoryGroup.Select(x => x.Isim).ToArray();
                ViewBag.CategoryCounts = categoryGroup.Select(x => x.Adet).ToArray();
            }
            else
            {
                ViewBag.CategoryLabels = new string[] { "Veri Yok" };
                ViewBag.CategoryCounts = new int[] { 0 };
            }

            // 4. ANALİZLER (Top Products)
            ViewBag.TopProducts = _productRepo.GetAll("Comments")?
                .OrderByDescending(p => p.Comments?.Count() ?? 0)
                .Take(5)
                .Select(p => new {
                    Name = p.Name,
                    TotalSales = (p.Comments?.Count() ?? 0) + 5,
                    Stock = p.Stock,
                    Price = p.Price
                }).ToList();

            // Kategori Satış Simülasyonu
            ViewBag.CategorySalesLabels = _categoryRepo.GetAll()?.Select(x => x.Name).ToArray() ?? new string[0];
            ViewBag.CategorySalesCounts = _categoryRepo.GetAll()?.Select(x => new Random().Next(10, 50)).ToArray() ?? new int[0];

            // Sayfanın altına son eklenen ürünleri model olarak gönderiyoruz
            var lastProducts = _productRepo.GetAll("Category")?
                                .OrderByDescending(p => p.CreatedDate)
                                .Take(5)
                                .ToList() ?? new List<Product>();

            return View(lastProducts);
        }
    }
}