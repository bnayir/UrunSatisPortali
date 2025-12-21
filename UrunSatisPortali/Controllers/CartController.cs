using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

public class CartController : Controller
{
    private readonly IRepository<Product> _productRepo;
    // Basitlik olması için sepeti Session yerine statik bir listede tutuyoruz 
    // (Gerçek projede Veritabanı veya Session tercih edilir)
    private static List<CartItem> _cart = new List<CartItem>();

    public CartController(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public IActionResult Index()
    {
        return View(_cart);
    }

    public IActionResult AddToCart(int id)
    {
        var product = _productRepo.GetById(id);
        if (product != null)
        {
            var item = _cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null) item.Quantity++;
            else _cart.Add(new CartItem { ProductId = id, ProductName = product.Name, Price = product.Price, Quantity = 1 });
        }
        return RedirectToAction("Index");
    }

    public IActionResult Remove(int id)
    {
        _cart.RemoveAll(x => x.ProductId == id);
        return RedirectToAction("Index");
    }
}

// Yardımcı Model (Models klasörüne de koyabilirsin)
public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}