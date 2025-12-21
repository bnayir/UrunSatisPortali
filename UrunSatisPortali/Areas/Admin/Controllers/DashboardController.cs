using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Linq;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Brand> _brandRepo; // Marka eklendiyse
        private readonly IRepository<Comment> _commentRepo; // Yorum eklendiyse

        public DashboardController(
            IRepository<Product> productRepo,
            IRepository<Category> categoryRepo,
            IRepository<Brand> brandRepo,
            IRepository<Comment> commentRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
            _commentRepo = commentRepo;
        }

        public IActionResult Index()
        {
            // 1. İstatistik Kutuları İçin Sayılar
            ViewBag.ProductCount = _productRepo.GetAll().Count();
            ViewBag.CategoryCount = _categoryRepo.GetAll().Count();
            ViewBag.BrandCount = _brandRepo.GetAll()?.Count() ?? 0;
            ViewBag.CommentCount = _commentRepo.GetAll()?.Count() ?? 0;

            // 2. GRAFİK VERİSİ: Kategori bazlı ürün dağılımı
            var categoryData = _productRepo.GetAll("Category")
                .GroupBy(p => p.Category.Name)
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.CategoryLabels = categoryData.Select(x => x.Label).ToArray();
            ViewBag.CategoryCounts = categoryData.Select(x => x.Count).ToArray();

            // 3. LİSTE: Son eklenen 5 ürünü getir
            var lastProducts = _productRepo.GetAll("Category")
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .ToList();

            return View(lastProducts); // Modeli View'a gönderiyoruz
        }
    }
}