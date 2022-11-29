namespace CategoryProducts.Shared
{
    using CategoryProducts.Constraints;
    using CategoryProducts.Services.User;
    using CategoryProducts.ViewModels.System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IOptions<JwtOptions> configuration;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtOptions> configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var jwtToken = this.GetJwtTokenValue(token);

                if (jwtToken != null)
                {
                    var isValid = await this.AppUserSecretKey(jwtToken, userService);

                    if (isValid)
                    {
                        await this.AttachUserToContextAsync(context, userService, jwtToken);
                    }
                }
            }

            await this.next(context);
        }

        private async Task<bool> AppUserSecretKey(JwtSecurityToken jwtToken, IUserService userService)
        {
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var auroraUserSecretKey = jwtToken.Claims.FirstOrDefault(x => x.Type == AppConstants.AppUserSecretKey);
            if (auroraUserSecretKey != null && userId != null)
            {
                return await userService.ValidateAuroraUserSecretKey(userId.Value, auroraUserSecretKey.Value);
            }

            return false;
        }

        private JwtSecurityToken? GetJwtTokenValue(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(this.configuration.Value.SecurityKey);
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidIssuer = this.configuration.Value.ValidIssuer,
                        ValidAudience = this.configuration.Value.ValidAudience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken;
            }
            catch
            {
                return null;
            }
        }

        private async Task AttachUserToContextAsync(HttpContext context, IUserService userService, JwtSecurityToken token)
        {
            try
            {
                var userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    context.Items["User"] = await userService.GetUserById(userId.Value);
                }
            }
            catch
            {
            }
        }
    }
}