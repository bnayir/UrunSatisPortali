using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Brand> _brandRepo; // EKLENDİ

        // Constructor'a _brandRepo eklendi
        public ProductController(IRepository<Product> productRepo, IRepository<Category> categoryRepo, IRepository<Brand> brandRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
        }

        public IActionResult Index()
        {
            // Hem Kategori hem Marka bilgilerini beraber çekiyoruz
            var products = _productRepo.GetAll("Category,Brand");
            return View(products);
        }

        public IActionResult Create()
        {
            // Kategorileri çek ve ViewBag'e koy
            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name");

            // MARKALARI ÇEK VE ViewBag'e KOY (Burası eksik olabilir)
            var brands = _brandRepo.GetAll();
            ViewBag.BrandId = new SelectList(brands, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                _productRepo.Add(product);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);
            ViewBag.BrandId = new SelectList(_brandRepo.GetAll(), "Id", "Name", product.BrandId); // Hata durumunda tekrar doldur
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);
            ViewBag.BrandId = new SelectList(_brandRepo.GetAll(), "Id", "Name", product.BrandId); // Markayı gönder
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            var dbProduct = _productRepo.GetById(product.Id);
            if (dbProduct == null) return NotFound();

            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;
            dbProduct.Price = product.Price;
            dbProduct.Stock = product.Stock;
            dbProduct.IsActive = product.IsActive;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.BrandId = product.BrandId; // Markayı güncelle
            dbProduct.Image = product.Image;
            dbProduct.UpdatedDate = DateTime.Now;

            _productRepo.Update(dbProduct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return Json(new { success = false });

            _productRepo.Delete(product);
            return Json(new { success = true });
        }
        // Ana projedeki ProductController içine:
        public IActionResult Details(int id)
        {
            // Ürünü, Kategorisini, Markasını ve Yorumlarını (Kullanıcılarıyla birlikte) çekiyoruz
            var product = _productRepo.GetAll("Category,Brand,Comments.User").FirstOrDefault(x => x.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
    }
}