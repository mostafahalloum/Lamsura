using System.ComponentModel.DataAnnotations;

namespace Lamsura.DTOs.Request
{
    public class ContactRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Subject { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}