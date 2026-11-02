using System.ComponentModel.DataAnnotations;

namespace Lamsura.DTOs.Request
{
    public class ApplicationUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }

    }
}
