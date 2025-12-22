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
        private readonly IRepository<Newsletter> _newsletterRepo;

        public HomeController(IRepository<Product> productRepo, IRepository<Category> categoryRepo, IRepository<Newsletter> newsletterRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _newsletterRepo = newsletterRepo;
        }

        // --- TEK BÝR INDEX METODUNDA ARAMA VE KATEGORÝ BÝRLEÞTÝRÝLDÝ ---
        public IActionResult Index(int? categoryId, string searchString)
        {
            // 1. Ana Ürün Sorgusu (Ýliþkili tablolarla birlikte)
            var productsQuery = _productRepo.GetAll("Category,Brand");

            // 2. ARAMA MANTIÐI
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(s => s.Name.Contains(searchString)
                                          || s.Description.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            // 3. KATEGORÝ FÝLTRELEME MANTIÐI
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

            // 4. Ýlginizi Çekebilecek Ürünler (Rastgele 4 Ürün)
            ViewBag.SuggestedProducts = _productRepo.GetAll("Category,Brand")
                                                     .OrderBy(x => Guid.NewGuid())
                                                     .Take(4)
                                                     .ToList();

            // 5. Sonuçlarý listele
            var products = productsQuery.OrderByDescending(x => x.Id).ToList();
            return View(products);
        }

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Lütfen bir e-posta adresi giriniz!" });
            }

            var isExist = _newsletterRepo.GetAll().Any(x => x.Email.ToLower() == email.ToLower());
            if (isExist)
            {
                return Json(new { success = false, message = "Bu e-posta adresi zaten kayýtlý!" });
            }

            try
            {
                var newsletter = new Newsletter { Email = email, CreatedDate = DateTime.Now };
                _newsletterRepo.Add(newsletter);
                return Json(new { success = true, message = "Bültenimize baþarýyla abone oldunuz!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Bir hata oluþtu!" });
            }
        }
    }
}