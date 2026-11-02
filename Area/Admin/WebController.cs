using Lamsura.DTOs.Request;
using Lamsura.DTOs.Response;
using Lamsura.Models;
using Lamsura.Repositories.IRepositories;
using Lamsura.Services;
using Lamsura.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Lamsura.Area.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    [Area("Admin")]
    public class WebController : ControllerBase
    {
        private readonly IRepository<web> _webrepository;
        private readonly IFileService _fileService;
        private const string FolderName = "Projects";

        public WebController ( IRepository<web> repository, IFileService fileService )
        {
            _webrepository = repository;
            _fileService = fileService;
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetAll ()
        {
            var data = await _webrepository.GetAsync(tracked: false);
            var response = data.Select(w => new WebResponse
            {
                Id = w.Id,
                Title = w.Title,
                Description = w.Description,
                Thumbnail = w.Thumbnail,
                DemoUrl = w.DemoUrl,
                Technologies = w.Technologies,
                CategoryId = w.CategoryId
            }).ToList();

            return Ok(new { Data = response });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetOne ( int id )
        {
            var w = await _webrepository.GetOneAsync(x => x.Id == id, tracked: false);
            if ( w == null )
                return NotFound();

            var response = new WebResponse
            {
                Id = w.Id,
                Title = w.Title,
                Description = w.Description,
                Thumbnail = w.Thumbnail,
                DemoUrl = w.DemoUrl,
                Technologies = w.Technologies,
                CategoryId = w.CategoryId
            };
            return Ok(new { Data = response });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Create ( [FromForm] WebRequest request )
        {
            var web = new web
            {
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId,
                DemoUrl = request.DemoUrl,
                Technologies = request.Technologies
            };

            if ( request.ThumbnailFile != null )
                web.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);

            await _webrepository.AddAsync(web);
            await _webrepository.CommitAsync();

            return CreatedAtAction(nameof(GetOne), new { id = web.Id }, new { success_notification = "Web Project Added Successfully" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Edit ( int id, [FromForm] WebRequest request )
        {
            var w = await _webrepository.GetOneAsync(x => x.Id == id);
            if ( w == null )
                return NotFound();

            w.Title = request.Title;
            w.Description = request.Description;
            w.CategoryId = request.CategoryId;
            w.DemoUrl = request.DemoUrl;
            w.Technologies = request.Technologies;

            if ( request.ThumbnailFile != null ) {
                _fileService.DeleteFile(w.Thumbnail, FolderName);
                w.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);
            }

            _webrepository.Update(w);
            await _webrepository.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var w = await _webrepository.GetOneAsync(x => x.Id == id);
            if ( w == null )
                return NotFound();

            _fileService.DeleteFile(w.Thumbnail, FolderName);
            _webrepository.Delete(w);
            await _webrepository.CommitAsync();
            return NoContent();
        }
    }
}