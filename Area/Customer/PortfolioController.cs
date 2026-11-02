using Lamsura.Data_Access;
using Lamsura.DTOs.Public;
using Lamsura.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lamsura.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PortfolioController ( ApplicationDbContext context )
        {
            _context = context;
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects ( string? type )
        {
            IQueryable<Project> query = _context.Projects.AsNoTracking().Include(x => x.Category);


            switch ( type?.ToLower() ) {
                case "web":
                    query = query.OfType<web>();
                    break;

                case "app":
                    query = query.OfType<app>();
                    break;

                case "video":
                    query = query.OfType<vedio>();
                    break;

                case "advertisement":
                    query = query.OfType<advertisement>();
                    break;
            }

            var projects = await query.ToListAsync();

            var result = projects.Select(p => new ProjectResponse
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Thumbnail = p.Thumbnail,
                Category = p.Category!.Name,

                DemoUrl = ( p as web )?.DemoUrl,

                VedioUrl =
                ( p as app )?.VedioUrl ??
                ( p as vedio )?.VedioUrl,

                Technologies =
                ( p as web )?.Technologies ??
                ( p as app )?.Technologies,

                ImgUrl = ( p as advertisement )?.ImgUrl,

            }).ToList();

            return Ok(result);
        }
    }
}