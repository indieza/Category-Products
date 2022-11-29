using CategoryProducts.Constraints;

using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

namespace CategoryProducts.Data.Models.User
{
    public class Role : IdentityRole
    {
        public Role()
        {
        }

        [Required]
        [Range(ModelConstraints.RoleMinLevel, ModelConstraints.RoleMaxLevel)]
        public int Level { get; set; }

        public virtual ICollection<UserRole> UsersRoles { get; set; } = new HashSet<UserRole>();
    }
}