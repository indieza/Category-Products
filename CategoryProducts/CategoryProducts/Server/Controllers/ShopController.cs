namespace CategoryProducts.Server.Controllers
{
    using CategoryProducts.InputModels.Shop;
    using CategoryProducts.InputModels.User;
    using CategoryProducts.Resources;
    using CategoryProducts.Services.Shop;
    using CategoryProducts.ViewModels.Shop;
    using CategoryProducts.ViewModels.System;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IShopService shopService;
        private readonly IStringLocalizer<Resource> localizer;

        public ShopController(IShopService shopService, IStringLocalizer<Resource> localizer)
        {
            this.shopService = shopService;
            this.localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await this.shopService.GetAllCategoriesAync();
            return this.Ok(result);
        }

        [HttpGet]
        [Route("categoryId")]
        public async Task<IActionResult> GetAllProductsByCategory([FromRoute] string categoryId)
        {
            var result = await this.shopService.GetAllProductsByCategoryAync(categoryId);
            return this.Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCategory([FromBody] CategoryInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                var username = this.User.Identity.Name;
                CompletedOperation<CategoryViewModel> result = await this.shopService.AddCategoryAsync(model, username);
                return result.Key == "Success" ? this.Ok(result) : this.BadRequest(result);
            }

            return this.BadRequest(new CompletedOperation<CategoryViewModel?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromBody] ProductInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                var username = this.User.Identity.Name;
                CompletedOperation<ProductViewModel?> result = await this.shopService.AddProductAsync(model, username);
                return result.Key == "Success" ? this.Ok(result) : this.BadRequest(result);
            }

            return this.BadRequest(new CompletedOperation<ProductViewModel?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditCategory([FromBody] EditCategoryInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                var username = this.User.Identity.Name;
                CompletedOperation<CategoryViewModel?> result = await this.shopService.EditCategoryAsync(model, username);
                return result.Key == "Success" ? this.Ok(result) : this.BadRequest(result);
            }

            return this.BadRequest(new CompletedOperation<CategoryViewModel?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProduct([FromBody] EditProductInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                var username = this.User.Identity.Name;
                CompletedOperation<ProductViewModel?> result = await this.shopService.EditProductAsync(model, username);
                return result.Key == "Success" ? this.Ok(result) : this.BadRequest(result);
            }

            return this.BadRequest(new CompletedOperation<ProductViewModel?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }
    }
}