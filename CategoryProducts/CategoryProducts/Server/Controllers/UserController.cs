namespace CategoryProducts.Server.Controllers
{
    using CategoryProducts.InputModels.User;
    using CategoryProducts.Resources;
    using CategoryProducts.Services.User;
    using CategoryProducts.ViewModels.System;
    using CategoryProducts.ViewModels.User;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IStringLocalizer<Resource> localizer;

        public UserController(IUserService userService, IStringLocalizer<Resource> localizer)
        {
            this.userService = userService;
            this.localizer = localizer;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.userService.RegisterAsync(model);
                if (result.Response == null)
                {
                    return this.BadRequest(result);
                }

                return this.Ok(result);
            }

            return this.BadRequest(new CompletedOperation<LoginInputModel?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.userService.LoginAsync(model);
                if (string.IsNullOrEmpty(result.Response))
                {
                    return this.Unauthorized(result);
                }

                return this.Ok(result);
            }

            return this.BadRequest(new CompletedOperation<string?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                CompletedOperation<UserViewModel?> result = await this.userService.GetUserProfile(username);
                if (result.Key == "Success")
                {
                    return this.Ok(result);
                }

                return this.BadRequest(result);
            }

            return this.BadRequest(new CompletedOperation<UserViewModel?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditUserProfile([FromBody] EditUserInputModel model)
        {
            var username = this.User.Identity?.Name;
            if (this.ModelState.IsValid && !string.IsNullOrEmpty(username))
            {
                CompletedOperation<UserViewModel?> result = await this.userService.EditUserProfile(model, username);
                if (result.Key == "Success")
                {
                    return this.Ok(result);
                }

                return this.BadRequest(result);
            }

            return this.BadRequest(new CompletedOperation<UserViewModel?>()
            {
                Key = "Error",
                Title = this.localizer["Error"],
                Message = this.localizer["Invalid input model"],
                Response = null,
            });
        }
    }
}