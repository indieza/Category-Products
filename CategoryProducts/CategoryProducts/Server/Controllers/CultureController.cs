namespace CategoryProducts.Server.Controllers
{
    using CategoryProducts.Resources;
    using CategoryProducts.ViewModels.System;

    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CultureController : ControllerBase
    {
        private readonly IStringLocalizer<Resource> localizer;

        public CultureController(IStringLocalizer<Resource> localizer)
        {
            this.localizer = localizer;
        }

        [HttpGet]
        [Route("{cultureName}")]
        public IActionResult SetCulture(string cultureName)
        {
            if (cultureName != null)
            {
                this.HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(cultureName)), new CookieOptions()
                        {
                            Expires = DateTimeOffset.UtcNow.AddMonths(1),
                        });
            }

            return this.Ok(new CompletedOperation<string?>()
            {
                Key = "Success",
                Title = this.localizer["Success"],
                Message = this.localizer["Successfully change website language"],
                Response = this.localizer["Successfully change website language"],
            });
        }
    }
}