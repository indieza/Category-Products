namespace CategoryProducts.Data.Models.Shop
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CategoryProducts.Models;

    using Models.User;

    public class Category : BaseDataModel
    {
        [MaxLength(60)]
        public string Name { get; set; }

        [Required]
        [ForeignKey(nameof(Models.User.User))]
        public string UserId { get; set; }

        public User User { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}