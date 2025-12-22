using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Models;
using UrunSatisPortali.Data;
using System.Linq;

namespace UrunSatisPortali.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;

        public HomeController(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index(int? categoryId)
        {
            // 1. Ana Ürün Sorgusu
            var productsQuery = _productRepo.GetAll("Category,Brand");

            // 2. Kategori ve Yan Menü Mantýðý
            if (categoryId.HasValue)
            {
                var subCategories = _categoryRepo.GetAll()
                                                 .Where(x => x.ParentId == categoryId.Value)
                                                 .ToList();

                if (subCategories.Any())
                {
                    // Ana kategori seçildiyse: Alt kategorilerin ürünlerini de getir
                    var subCategoryIds = subCategories.Select(s => s.Id).ToList();
                    subCategoryIds.Add(categoryId.Value);

                    productsQuery = productsQuery.Where(x => subCategoryIds.Contains(x.CategoryId));
                    ViewBag.Categories = subCategories;
                }
                else
                {
                    // Alt kategori seçildiyse: Sadece o kategoriyi getir
                    productsQuery = productsQuery.Where(x => x.CategoryId == categoryId.Value);
                    ViewBag.Categories = _categoryRepo.GetAll().Where(x => x.ParentId == null).ToList();
                }
            }
            else
            {
                ViewBag.Categories = _categoryRepo.GetAll().Where(x => x.ParentId == null).ToList();
            }

            ViewBag.ActiveCategory = categoryId;

            // 3. Ýlginizi Çekebilecek Ürünler (Rastgele 4 Ürün)
            // Not: Mevcut sayfada filtrelenen ürünlerden farklý olmasý için OrderBy(Guid) kullanýyoruz.
            ViewBag.SuggestedProducts = _productRepo.GetAll("Category,Brand")
                                                    .OrderBy(x => Guid.NewGuid())
                                                    .Take(4)
                                                    .ToList();

            // 4. Ürünleri Id'ye göre sýrala ve gönder
            var products = productsQuery.OrderByDescending(x => x.Id).ToList();
            return View(products);
        }
    }
}