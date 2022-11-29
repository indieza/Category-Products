namespace CategoryProducts.Services.Shop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using CategoryProducts.Data;
    using CategoryProducts.Data.Models.Shop;
    using CategoryProducts.Data.Models.User;
    using CategoryProducts.InputModels.Shop;
    using CategoryProducts.ViewModels.Shop;
    using CategoryProducts.ViewModels.System;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class ShopService : IShopService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public ShopService(Context db, IMapper mapper, UserManager<User> userManager)
        {
            this.db = db;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<CompletedOperation<CategoryViewModel>> AddCategoryAsync(CategoryInputModel model, string username)
        {
            var currnetUser = await this.userManager.FindByNameAsync(username);
            var target = new Category()
            {
                Name = model.Name,
                UserId = currnetUser.Id,
            };
            await this.db.Categories.AddAsync(target);
            await this.db.SaveChangesAsync();
            return new CompletedOperation<CategoryViewModel?>()
            {
                Key = "Success",
                Title = "Success",
                Message = "Successfully add category",
                Response = await this.GetCategoryByIdAsync(target.Id),
            };
        }

        public async Task<CompletedOperation<ProductViewModel?>> AddProductAsync(ProductInputModel model, string username)
        {
            var currnetUser = await this.userManager.FindByNameAsync(username);
            var targetCategory = await this.db.Categories.FirstOrDefaultAsync(x => x.Id == model.CategoryId);
            if (targetCategory != null)
            {
                var target = new Product()
                {
                    Price = model.Price,
                    Name = model.Name,
                    UserId = currnetUser.Id,
                    CategoryId = targetCategory.Id,
                };

                await this.db.Products.AddAsync(target);
                await this.db.SaveChangesAsync();

                return new CompletedOperation<ProductViewModel?>()
                {
                    Key = "Success",
                    Title = "Success",
                    Message = "Successfully add product",
                    Response = await this.GetProductByIdAsync(target.Id),
                };
            }

            return new CompletedOperation<ProductViewModel?>()
            {
                Key = "Error",
                Title = "Error",
                Message = "Category does not exist",
                Response = null,
            };
        }

        public async Task<CompletedOperation<CategoryViewModel?>> EditCategoryAsync(EditCategoryInputModel model, string username)
        {
            var targetCategory = await this.db.Categories
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.User.UserName == username);
            if (targetCategory != null)
            {
                targetCategory.Name = model.Name;
                this.db.Categories.Update(targetCategory);
                await this.db.SaveChangesAsync();
                return new CompletedOperation<CategoryViewModel?>()
                {
                    Key = "Success",
                    Title = "Success",
                    Message = "Successfully edit category",
                    Response = await this.GetCategoryByIdAsync(targetCategory.Id),
                };
            }

            return new CompletedOperation<CategoryViewModel?>()
            {
                Key = "Error",
                Title = "Error",
                Message = "Category does not exist",
                Response = null,
            };
        }

        public async Task<CompletedOperation<ProductViewModel?>> EditProductAsync(EditProductInputModel model, string username)
        {
            var targetCategory = await this.db.Categories
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.User.UserName == username);
            var targetProduct = await this.db.Products
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.CategoryId == model.CategoryId && x.User.UserName == username);

            if (targetCategory == null)
            {
                return new CompletedOperation<ProductViewModel?>()
                {
                    Key = "Error",
                    Title = "Error",
                    Message = "Category does not exist",
                    Response = null,
                };
            }

            if (targetProduct == null)
            {
                return new CompletedOperation<ProductViewModel?>()
                {
                    Key = "Error",
                    Title = "Error",
                    Message = "Product does not exist",
                    Response = null,
                };
            }

            targetProduct.Name = model.Name;
            targetProduct.Price = model.Price;
            targetProduct.CategoryId = model.CategoryId;

            this.db.Products.Update(targetProduct);
            await this.db.SaveChangesAsync();
            return new CompletedOperation<ProductViewModel?>()
            {
                Key = "Success",
                Title = "Success",
                Message = "Successfully edit product",
                Response = await this.GetProductByIdAsync(targetProduct.Id),
            };
        }

        public async Task<CompletedOperation<List<CategoryViewModel>>> GetAllCategoriesAync()
        {
            var data = await this.db.Categories
                .Include(x => x.User)
                .OrderByDescending(x => x.CreateOn)
                .AsNoTracking()
                .ToListAsync();

            return new CompletedOperation<List<CategoryViewModel>>()
            {
                Key = "Success",
                Title = "Success",
                Message = "Successfull get all categories",
                Response = this.mapper.Map<List<CategoryViewModel>>(data),
            };
        }

        public async Task<CompletedOperation<List<ProductViewModel>>> GetAllProductsByCategoryAync(string categoryId)
        {
            var data = await this.db.Products
                .Where(x => x.CategoryId == categoryId)
                .Include(x => x.User)
                .Include(x => x.Category)
                .OrderByDescending(x => x.CreateOn)
                .AsNoTracking()
                .ToListAsync();

            return new CompletedOperation<List<ProductViewModel>>()
            {
                Key = "Success",
                Title = "Success",
                Message = "Successfull get all products by category",
                Response = this.mapper.Map<List<ProductViewModel>>(data),
            };
        }

        private async Task<CategoryViewModel> GetCategoryByIdAsync(string id)
        {
            var target = await this.db.Categories
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return this.mapper.Map<CategoryViewModel>(target);
        }

        private async Task<ProductViewModel> GetProductByIdAsync(string id)
        {
            var target = await this.db.Products
                .Include(x => x.User)
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return this.mapper.Map<ProductViewModel>(target);
        }
    }
}