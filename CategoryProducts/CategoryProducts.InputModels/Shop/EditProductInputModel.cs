namespace CategoryProducts.InputModels.Shop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditProductInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(60)]
        public string Name { get; set; }

        [Required]
        [Range(0.1, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        public string CategoryId { get; set; }
    }
}