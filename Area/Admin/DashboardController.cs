using Lamsura.Data_Access;
using Lamsura.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lamsura.Area.Admin
{

    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    [Area("Admin")]
    public class DashboardController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public DashboardController ( ApplicationDbContext context )
        {
            _context = context;
        }

        [HttpGet("stats")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetDashboardStats ()
        {
            var stats = new
            {
                TotalProjects = await _context.Projects.CountAsync(),
                NewMessages = await _context.Messages.CountAsync(m => !m.IsRead),
                CategoriesCount = await _context.Categories.CountAsync(),
                RecentProjects = await _context.Projects
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new { p.Id, p.Title, p.Thumbnail })
                    .ToListAsync()
            };

            return Ok(stats);

        }
    }
}
