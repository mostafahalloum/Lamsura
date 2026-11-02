namespace Lamsura.DTOs.Public
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? DemoUrl { get; set; }
        public string? VedioUrl { get; set; }
        public string? ImgUrl { get; set; }
        public string? Technologies { get; set; }
    }
}