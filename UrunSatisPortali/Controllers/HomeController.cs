using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Models;
using UrunSatisPortali.Data;

public class HomeController : Controller
{
    private readonly IRepository<Product> _productRepo;

    public HomeController(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public IActionResult Index()
    {
        // Veritabanýndaki tüm ürünleri alýp ana sayfaya (View) gönderiyoruz
        var products = _productRepo.GetAll().ToList();
        return View(products);
    }
}