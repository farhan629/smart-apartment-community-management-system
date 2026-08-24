using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Features.Auth.DTOs
{
    public class RegisterRequestDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public Guid? Role_id { get; set; }
        public Guid? Flat_id { get; set; }
        public IFormFile? Photo { get; set; }
    }

    public class RegisterManagementRequestDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public Guid? Role_id { get; set; }
        public Guid? category_id { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
