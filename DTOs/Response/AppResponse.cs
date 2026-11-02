namespace Lamsura.DTOs.Response
{
    public class AppResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Technologies { get; set; }
        public string? VideoUrl { get; set; }
        public string? Thumbnail { get; set; }
        public int CategoryId { get; set; }
    }
}
