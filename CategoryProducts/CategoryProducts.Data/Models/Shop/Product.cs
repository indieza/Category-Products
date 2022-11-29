namespace CategoryProducts.Data.Models.Shop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CategoryProducts.Data.Models.User;
    using CategoryProducts.Models;

    public class Product : BaseDataModel
    {
        [MaxLength(60)]
        public string Name { get; set; }

        [Range(0.1, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        [ForeignKey(nameof(Shop.Category))]
        public string CategoryId { get; set; }

        public Category Category { get; set; }

        [Required]
        [ForeignKey(nameof(Models.User.User))]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}