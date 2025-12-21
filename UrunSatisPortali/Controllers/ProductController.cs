using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepo;

        public ProductController(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        public IActionResult Details(int id)
        {
            // Verileri çekerken yorumları ve kullanıcıları da (Include) çekiyoruz
            var product = _productRepo.GetAll("Category,Brand,Comments,Comments.User")
                                      .FirstOrDefault(x => x.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
    }
}