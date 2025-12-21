using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

[Authorize]
public class CommentController : Controller
{
    private readonly IRepository<Comment> _commentRepo;

    public CommentController(IRepository<Comment> commentRepo)
    {
        _commentRepo = commentRepo;
    }

    [HttpPost]
    public IActionResult AddComment(int productId, string content)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Giriş yapan kullanıcının ID'si

        var comment = new Comment
        {
            ProductId = productId,
            Content = content,
            UserId = userId,
            CreatedDate = DateTime.Now
        };

        _commentRepo.Add(comment);
        return RedirectToAction("Details", "Product", new { id = productId });
    }
}