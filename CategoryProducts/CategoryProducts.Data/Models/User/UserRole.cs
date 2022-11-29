using Microsoft.AspNetCore.Identity;

namespace CategoryProducts.Data.Models.User
{
    public class UserRole : IdentityUserRole<string>
    {
        public UserRole()
        {
        }

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}