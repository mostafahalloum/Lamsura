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
    public class AdvertisementController : ControllerBase
    {
        private readonly IRepository<advertisement> _adRepository;
        private readonly IFileService _fileService;
        private const string FolderName = "Projects";

        public AdvertisementController ( IRepository<advertisement> repository, IFileService fileService )
        {
            _adRepository = repository;
            _fileService = fileService;
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> GetAll ()
        {
            var data = await _adRepository.GetAsync(tracked: false);
            var response = data.Select(a => new AdvertisementResponse
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Thumbnail = a.Thumbnail,
                Img = a.ImgUrl,      
                CategoryId = a.CategoryId
            }).ToList();

            return Ok(new { Data = response });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne ( int id )
        {
            var a = await _adRepository.GetOneAsync(x => x.Id == id, tracked: false);
            if ( a == null )
                return NotFound();

            var response = new AdvertisementResponse
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Thumbnail = a.Thumbnail,
                Img = a.ImgUrl,
                CategoryId = a.CategoryId
            };
            return Ok(new { Data = response });
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromForm] AdvertisementRequest request )
        {
            var ad = new advertisement
            {
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId
            };

       
            if ( request.ThumbnailFile != null )
                ad.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);

            if ( request.ImgFile != null )
                ad.ImgUrl = await _fileService.SaveFileAsync(request.ImgFile, FolderName);

            await _adRepository.AddAsync(ad);
            await _adRepository.CommitAsync();

            return CreatedAtAction(nameof(GetOne), new { id = ad.Id }, new { success_notification = "Advertisement Added Successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit ( int id, [FromForm] AdvertisementRequest request )
        {
            var a = await _adRepository.GetOneAsync(x => x.Id == id);
            if ( a == null )
                return NotFound();

            a.Title = request.Title;
            a.Description = request.Description;
            a.CategoryId = request.CategoryId;

            if ( request.ThumbnailFile != null ) {
                _fileService.DeleteFile(a.Thumbnail, FolderName);
                a.Thumbnail = await _fileService.SaveFileAsync(request.ThumbnailFile, FolderName);
            }

            if ( request.ImgFile != null ) {
                _fileService.DeleteFile(a.ImgUrl, FolderName);
                a.ImgUrl = await _fileService.SaveFileAsync(request.ImgFile, FolderName);
            }

            _adRepository.Update(a);
            await _adRepository.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var a = await _adRepository.GetOneAsync(x => x.Id == id);
            if ( a == null )
                return NotFound();

           
            _fileService.DeleteFile(a.Thumbnail, FolderName);
            _fileService.DeleteFile(a.ImgUrl, FolderName);

            _adRepository.Delete(a);
            await _adRepository.CommitAsync();
            return NoContent();
        }
    }
}