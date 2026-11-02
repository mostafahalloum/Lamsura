namespace Lamsura.DTOs.Response
{
    public class WebResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }   
        public string? DemoUrl { get; set; }  
        public string? Technologies { get; set; } 
        public int CategoryId { get; set; }
    }
}
