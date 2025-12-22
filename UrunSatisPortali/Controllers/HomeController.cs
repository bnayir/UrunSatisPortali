using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Models;
using UrunSatisPortali.Data;

public class HomeController : Controller
{
    private readonly IRepository<Product> _productRepo;
    private readonly IRepository<Category> _categoryRepo; // Kategori repository eklendi

    public HomeController(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
    }

    // Parametre olarak categoryId alýyoruz (Filtreleme için)
    public IActionResult Index(int? categoryId)
    {
        // 1. Yan menüdeki kategorileri doldurmak için tüm kategorileri gönderiyoruz
        ViewBag.Categories = _categoryRepo.GetAll().ToList();

        // 2. Hangi kategorinin seçili olduðunu View'da "mavi" yapmak için tutuyoruz
        ViewBag.ActiveCategory = categoryId;

        // 3. Ürünleri Marka ve Kategori bilgileriyle birlikte çekiyoruz (Include mantýðý)
        var productsQuery = _productRepo.GetAll("Category,Brand");

        // 4. Eðer bir kategori seçilmiþse, ürünleri o kategoriye göre filtreliyoruz
        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(x => x.CategoryId == categoryId.Value);
        }

        var products = productsQuery.ToList();
        return View(products);
    }
}