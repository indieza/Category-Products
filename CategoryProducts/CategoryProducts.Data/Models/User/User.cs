using CategoryProducts.Constraints;
using CategoryProducts.Data.Models.Shop;

using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

namespace CategoryProducts.Data.Models.User
{
    public class User : IdentityUser
    {
        public User()
        {
            this.AppUserSecretKey = $"{Guid.NewGuid()}_{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds()}";
        }

        [Required]
        public string AppUserSecretKey { get; set; }

        [StringLength(ModelConstraints.NameMaxLength, MinimumLength = ModelConstraints.NameMinLength)]
        public string? FirstName { get; set; }

        [StringLength(ModelConstraints.NameMaxLength, MinimumLength = ModelConstraints.NameMinLength)]
        public string? LastName { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }
}