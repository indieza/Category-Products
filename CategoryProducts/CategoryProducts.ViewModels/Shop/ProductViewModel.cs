namespace CategoryProducts.ViewModels.Shop
{
    using CategoryProducts.ViewModels.User;

    public class ProductViewModel
    {
        public string Name { get; set; }

        public double Price { get; set; }

        public string CategoryId { get; set; }

        public CategoryViewModel Category { get; set; }

        public string UserId { get; set; }

        public UserViewModel User { get; set; }
    }
}