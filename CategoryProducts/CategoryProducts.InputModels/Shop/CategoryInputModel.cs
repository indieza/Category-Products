namespace CategoryProducts.InputModels.Shop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CategoryInputModel
    {
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
    }
}