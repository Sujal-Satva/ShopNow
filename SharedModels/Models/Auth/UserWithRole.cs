using Microsoft.AspNetCore.Identity;
namespace SharedModels.Models.Auth
{
    public class UserWithRole : IdentityUser
    {
        public string RoleName { get; set; }
    }
}
