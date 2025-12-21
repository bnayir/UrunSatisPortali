using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Linq;
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
            // ANALİZ: En çok yorum alan (en popüler) 5 ürün
            var topProducts = _productRepo.GetAll("Comments")
                .OrderByDescending(p => p.Comments.Count())
                .Take(5)
                .Select(p => new {
                    Name = p.Name,
                    TotalSales = p.Comments.Count() + 5, // Satış tablon olmadığı için yorum + sabit sayı ile simüle ediyoruz
                    Stock = p.Stock,
                    Price = p.Price
                }).ToList();

            ViewBag.TopProducts = topProducts;

            // Kategori bazlı satış simülasyonu
            ViewBag.CategorySalesLabels = _categoryRepo.GetAll().Select(x => x.Name).ToArray();
            ViewBag.CategorySalesCounts = _categoryRepo.GetAll().Select(x => new Random().Next(10, 50)).ToArray();

            return View(_productRepo.GetAll("Category").OrderByDescending(p => p.CreatedDate).Take(5).ToList());
        }

        
    }
}