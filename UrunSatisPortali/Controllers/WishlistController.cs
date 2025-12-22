using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using Newtonsoft.Json;

public class WishlistController : Controller
{
    private readonly IRepository<Product> _productRepo;
    private const string WishlistSessionKey = "WishlistSession";

    public WishlistController(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public IActionResult Index()
    {
        var wishlist = GetWishlistFromSession();
        return View(wishlist);
    }

    public IActionResult AddToWishlist(int id)
    {
        var product = _productRepo.GetById(id);
        if (product != null)
        {
            var wishlist = GetWishlistFromSession();
            // Eğer ürün zaten favorilerde yoksa ekle
            if (!wishlist.Any(x => x.Id == id))
            {
                wishlist.Add(product);
                SaveWishlistToSession(wishlist);
            }
            string referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer)) return Redirect(referer);
        }
        return RedirectToAction("Index", "Home");
        
    }

    public IActionResult Remove(int id)
    {
        var wishlist = GetWishlistFromSession();
        wishlist.RemoveAll(x => x.Id == id);
        SaveWishlistToSession(wishlist);
        return RedirectToAction("Index");
    }

    private List<Product> GetWishlistFromSession()
    {
        var jsonStr = HttpContext.Session.GetString(WishlistSessionKey);
        return jsonStr == null ? new List<Product>() : JsonConvert.DeserializeObject<List<Product>>(jsonStr);
    }

    private void SaveWishlistToSession(List<Product> wishlist)
    {
        var jsonStr = JsonConvert.SerializeObject(wishlist);
        HttpContext.Session.SetString(WishlistSessionKey, jsonStr);
    }
}