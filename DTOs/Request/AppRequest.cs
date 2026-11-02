namespace Lamsura.DTOs.Request
{
    public class AppRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? Technologies { get; set; }
        public IFormFile? VideoFile { get; set; } 
        public IFormFile? ThumbnailFile { get; set; }
    }
}
