namespace CategoryProducts.ViewModels.Shop
{
    using global::System.ComponentModel.DataAnnotations;
    using global::System.ComponentModel.DataAnnotations.Schema;

    using CategoryProducts.ViewModels.User;

    public class CategoryViewModel : BaseDataViewModel
    {
        public string Name { get; set; }

        public string UserId { get; set; }

        public UserViewModel User { get; set; }

        public virtual ICollection<ProductViewModel> Products { get; set; } = new HashSet<ProductViewModel>();
    }
}