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
    public class AppController : ControllerBase
    {
        private readonly IRepository<app> _appRepository;
        private readonly IFileService _fileService;
        private const string FolderName = "Projects";

        public AppController ( IRepository<app> repository, IFileService fileService )
        {
            _appRepository = repository;
            _fileService = fileService;
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetAll ()
        {
            var data = await _appRepository.GetAsync(tracked: false);
            var response = data.Select(a => new AppResponse
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Technologies = a.Technologies,
                VideoUrl = a.VedioUrl,
                Thumbnail = a.Thumbnail,
                CategoryId = a.CategoryId
            }).ToList();

            return Ok(new { Data = response });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne ( int id )
        {
            var a = await _appRepository.GetOneAsync(x => x.Id == id, tracked: false);
            if ( a == null )
                return NotFound();

            var response = new AppResponse
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Technologies = a.Technologies,
                VideoUrl = a.VedioUrl,
                Thumbnail = a.Thumbnail, 
                CategoryId = a.CategoryId
            };
            return Ok(new { Data = response });
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromForm] AppRequest request )
        {
            var appItem = new app
            {
                Title = request.Title,
                Description = request.Description,
                Technologies = request.Technologies,
                CategoryId = request.CategoryId
            };

           
            if ( request.ThumbnailFile != null )
                appItem.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);

        
            if ( request.VideoFile != null )
                appItem.VedioUrl = await _fileService.SaveFileAsync(request.VideoFile, FolderName);

            await _appRepository.AddAsync(appItem);
            await _appRepository.CommitAsync();

            return CreatedAtAction(nameof(GetOne), new { id = appItem.Id }, new { success_notification = "App Project Added Successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit ( int id, [FromForm] AppRequest request )
        {
            var a = await _appRepository.GetOneAsync(x => x.Id == id);
            if ( a == null )
                return NotFound();

            a.Title = request.Title;
            a.Description = request.Description;
            a.Technologies = request.Technologies;
            a.CategoryId = request.CategoryId;

       
            if ( request.ThumbnailFile != null ) {
                _fileService.DeleteFile(a.Thumbnail, FolderName);
                a.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);
            }

            if ( request.VideoFile != null ) {
                _fileService.DeleteFile(a.VedioUrl, FolderName);
                a.VedioUrl = await _fileService.SaveFileAsync(request.VideoFile, FolderName);
            }

            _appRepository.Update(a);
            await _appRepository.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var a = await _appRepository.GetOneAsync(x => x.Id == id);
            if ( a == null )
                return NotFound();

            _fileService.DeleteFile(a.Thumbnail, FolderName);
            _fileService.DeleteFile(a.VedioUrl, FolderName);

            _appRepository.Delete(a);
            await _appRepository.CommitAsync();
            return NoContent();
        }
    }
}