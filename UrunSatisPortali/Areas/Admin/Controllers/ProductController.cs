using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
       
        
            private readonly IRepository<Product> _productRepo;
            private readonly IRepository<Category> _categoryRepo;

            public ProductController(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
            {
                _productRepo = productRepo;
                _categoryRepo = categoryRepo;
            }

            // --- LİSTELEME ---
            public IActionResult Index()
            {
                // Kategorileri dahil ederek getir (Repository güncellemesi sayesinde çalışır)
                var products = _productRepo.GetAll("Category");
                return View(products);
            }

            // --- EKLEME (GET) ---
            public IActionResult Create()
            {
                // Dropdown için kategorileri dolduruyoruz
                ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name");
                return View();
            }

            // --- EKLEME (POST) ---
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Create(Product product)
            {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
                {
                    product.CreatedDate = DateTime.Now;
                    _productRepo.Add(product);

                    // Başarı Mesajı
                    TempData["success"] = "Ürün başarıyla oluşturuldu!";

                    return RedirectToAction(nameof(Index));
                }

                // Hata varsa listeyi tekrar doldur (Yoksa sayfa bozuk görünür)
                ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);
                return View(product);
            }

            // --- DÜZENLEME (GET) ---
            public IActionResult Edit(int id)
            {
                var product = _productRepo.GetById(id);
                if (product == null) return NotFound();

                ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);
                return View(product);
            }

            // --- DÜZENLEME (POST) ---
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(Product product)
            {
                // 1. Veritabanındaki gerçek kaydı bul
                var dbProduct = _productRepo.GetById(product.Id);
                if (dbProduct == null) return NotFound();

                // 2. Yeni bilgileri üzerine yaz
                dbProduct.Name = product.Name;
                dbProduct.Description = product.Description;
                dbProduct.Price = product.Price;
                dbProduct.Stock = product.Stock;
                dbProduct.IsActive = product.IsActive;
                dbProduct.CategoryId = product.CategoryId;
                dbProduct.Image = product.Image; // URL Resim
                dbProduct.UpdatedDate = DateTime.Now;

                // 3. Kaydet
                _productRepo.Update(dbProduct);

                // Başarı Mesajı
                TempData["success"] = "Ürün başarıyla güncellendi!";

                return RedirectToAction(nameof(Index));
            }

            // --- SİLME (AJAX İLE) ---
            [HttpPost]
            public IActionResult Delete(int id)
            {
                var product = _productRepo.GetById(id);
                if (product == null) return Json(new { success = false, message = "Ürün bulunamadı" });

                _productRepo.Delete(product);
                return Json(new { success = true, message = "Ürün silindi" });
            }
        }
    }
// Bu, sınıfı kapatan parantez
// Bu, namespace'i kapatan parantez