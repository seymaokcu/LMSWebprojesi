using Microsoft.AspNetCore.Identity;
namespace LmsProje.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}