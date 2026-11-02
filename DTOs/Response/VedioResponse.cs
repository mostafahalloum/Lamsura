namespace Lamsura.DTOs.Response
{
    public class VedioResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Vedio { get; set; }
        public string? Thumbnail { get; set; }
        public int CategoryId { get; set; }
    }
}
