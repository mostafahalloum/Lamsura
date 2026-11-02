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
    public class VedioController : ControllerBase
    {
        private readonly IRepository<vedio> _vedioRepository;
        private readonly IFileService _fileService;
        private const string FolderName = "Projects";

        public VedioController ( IRepository<vedio> repository, IFileService fileService )
        {
            _vedioRepository = repository;
            _fileService = fileService;
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetAll ()
        {
            var data = await _vedioRepository.GetAsync(tracked: false);
            var response = data.Select(v => new VedioResponse
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                Thumbnail = v.Thumbnail,
                Vedio = v.VedioUrl,
                CategoryId = v.CategoryId
            }).ToList();

            return Ok(new { Data = response });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetOne ( int id )
        {
            var v = await _vedioRepository.GetOneAsync(x => x.Id == id, tracked: false);
            if ( v == null )
                return NotFound();

            var response = new VedioResponse
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                Thumbnail = v.Thumbnail,
                Vedio = v.VedioUrl,
                CategoryId = v.CategoryId
            };
            return Ok(new { Data = response });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Create ( [FromForm] VedioRequest request )
        {
            var vedioItem = new vedio
            {
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId
            };

        
            if ( request.ThumbnailFile != null )
                vedioItem.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);

        
            if ( request.VideoFile != null )
                vedioItem.VedioUrl = await _fileService.SaveFileAsync(request.VideoFile, FolderName);

            await _vedioRepository.AddAsync(vedioItem);
            await _vedioRepository.CommitAsync();

            return CreatedAtAction(nameof(GetOne), new { id = vedioItem.Id }, new { success_notification = "Video Project Added Successfully" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Edit ( int id, [FromForm] VedioRequest request )
        {
            var v = await _vedioRepository.GetOneAsync(x => x.Id == id);
            if ( v == null )
                return NotFound();

            v.Title = request.Title;
            v.Description = request.Description;
            v.CategoryId = request.CategoryId;

            if ( request.ThumbnailFile != null ) {
                _fileService.DeleteFile(v.Thumbnail, FolderName);
                v.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);
            }

            if ( request.VideoFile != null ) {
                _fileService.DeleteFile(v.VedioUrl, FolderName);
                v.VedioUrl = await _fileService.SaveFileAsync(request.VideoFile, FolderName);
            }

            _vedioRepository.Update(v);
            await _vedioRepository.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var v = await _vedioRepository.GetOneAsync(x => x.Id == id);
            if ( v == null )
                return NotFound();

            _fileService.DeleteFile(v.Thumbnail, FolderName);
            _fileService.DeleteFile(v.VedioUrl, FolderName);

            _vedioRepository.Delete(v);
            await _vedioRepository.CommitAsync();
            return NoContent();
        }
    }
}