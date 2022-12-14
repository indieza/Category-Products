namespace CategoryProducts.InputModels.Shop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditCategoryInputModel
    {
        [Required]
        public string Id { get; set; }

        [MaxLength(60)]
        public string Name { get; set; }
    }
}