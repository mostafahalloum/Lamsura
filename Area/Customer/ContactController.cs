using Lamsura.API.Models;
using Lamsura.Data_Access;
using Lamsura.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ContactController ( ApplicationDbContext context )
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Send ( ContactRequest request )
    {
        var message = new Message
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Content = request.Content,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Your message has been sent successfully."
        });
    }
}