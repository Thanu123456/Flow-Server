using Microsoft.AspNetCore.Identity;

namespace Flow_Api.Models.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() 
        {
            Description = string.Empty;
        }

        public ApplicationRole(string roleName) : base(roleName) 
        {
            Description = string.Empty;
        }

        public string Description { get; set; }
    }
}
