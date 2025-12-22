using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Models;
using UrunSatisPortali.Data;
using System.Linq;
using System;
using System.Collections.Generic;

namespace UrunSatisPortali.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Newsletter> _newsletterRepo; // Yeni e-bülten servisi

        public HomeController(IRepository<Product> productRepo, IRepository<Category> categoryRepo, IRepository<Newsletter> newsletterRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _newsletterRepo = newsletterRepo; // Dependency injection ile baðladýk
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
                    var subCategoryIds = subCategories.Select(s => s.Id).ToList();
                    subCategoryIds.Add(categoryId.Value);

                    productsQuery = productsQuery.Where(x => subCategoryIds.Contains(x.CategoryId));
                    ViewBag.Categories = subCategories;
                }
                else
                {
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
            ViewBag.SuggestedProducts = _productRepo.GetAll("Category,Brand")
                                                    .OrderBy(x => Guid.NewGuid())
                                                    .Take(4)
                                                    .ToList();

            // 4. Ürünleri Id'ye göre sýrala ve gönder
            var products = productsQuery.OrderByDescending(x => x.Id).ToList();
            return View(products);
        }

        // --- E-BÜLTEN ABONE OLMA METODU ---
        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Lütfen bir e-posta adresi giriniz!" });
            }

            // Daha önce kayýt olmuþ mu kontrolü
            var isExist = _newsletterRepo.GetAll().Any(x => x.Email.ToLower() == email.ToLower());
            if (isExist)
            {
                return Json(new { success = false, message = "Bu e-posta adresi zaten kayýtlý!" });
            }

            try
            {
                var newsletter = new Newsletter
                {
                    Email = email,
                    CreatedDate = DateTime.Now
                };

                _newsletterRepo.Add(newsletter);
                return Json(new { success = true, message = "Bültenimize baþarýyla abone oldunuz. Teþekkürler!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Bir hata oluþtu, lütfen daha sonra tekrar deneyiniz." });
            }
        }
    }
}