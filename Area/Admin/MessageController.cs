using Lamsura.Data_Access;
using Lamsura.DTOs.Response;
using Lamsura.Models;
using Lamsura.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lamsura.Area.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    [Area("Admin")]
    public class MessageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessageController ( ApplicationDbContext context )
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetAll ()
        {
            var messages = await _context.Messages
                .AsNoTracking()
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            var result = messages.Select(m => new MessageResponse
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Subject = m.Subject,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt
            });

            return Ok(new
            {
                Data = result
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetOne ( int id )
        {
            var message = await _context.Messages.FindAsync(id);
            if ( message == null )
                return NotFound();

            if ( !message.IsRead ) {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return Ok(new MessageResponse
            {
                Id = message.Id,
                Name = message.Name,
                Email = message.Email,
                Subject = message.Subject,
                Content = message.Content,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt
            });
        }

  
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var message = await _context.Messages.FindAsync(id);
            if ( message == null )
                return NotFound();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("unread-count")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> UnreadCount ()
        {
            var count = await _context.Messages
                .CountAsync(x => !x.IsRead);

            return Ok(count);
        }
    }
}