namespace CategoryProducts.Services.Shop
{
    using CategoryProducts.InputModels.Shop;
    using CategoryProducts.ViewModels.Shop;
    using CategoryProducts.ViewModels.System;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IShopService
    {
        Task<CompletedOperation<CategoryViewModel>> AddCategoryAsync(CategoryInputModel model, string username);

        Task<CompletedOperation<ProductViewModel?>> AddProductAsync(ProductInputModel model, string username);

        Task<CompletedOperation<CategoryViewModel?>> EditCategoryAsync(EditCategoryInputModel model, string username);

        Task<CompletedOperation<ProductViewModel?>> EditProductAsync(EditProductInputModel model, string username);

        Task<CompletedOperation<List<CategoryViewModel>>> GetAllCategoriesAync();

        Task<CompletedOperation<List<ProductViewModel>>> GetAllProductsByCategoryAync(string categoryId);
    }
}