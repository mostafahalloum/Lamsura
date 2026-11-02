namespace Lamsura.DTOs.Request
{
    public class AdvertisementRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public IFormFile? ImgFile { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
    }
}
