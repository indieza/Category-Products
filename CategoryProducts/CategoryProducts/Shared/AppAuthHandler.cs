namespace CategoryProducts.Shared
{
    using CategoryProducts.Constraints;
    using CategoryProducts.Services.User;
    using CategoryProducts.ViewModels.System;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Net.Http.Headers;

    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.RegularExpressions;

    public class AppAuthHandler : AuthenticationHandler<AppAuthSchemeOptions>
    {
        private readonly IOptions<JwtOptions> configuration;
        private readonly IUserService userService;

        public AppAuthHandler(IOptionsMonitor<AppAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IOptions<JwtOptions> configuration, IUserService userService)
            : base(options, logger, encoder, clock)
        {
            this.configuration = configuration;
            this.userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return await Task.FromResult(AuthenticateResult.Fail("App auth header not found."));
            }

            var header = this.Request.Headers[HeaderNames.Authorization].ToString();
            var tokenMatch = Regex.Match(header, AppConstants.AppAuhtHeader);

            if (tokenMatch.Success)
            {
                var token = header.Split(" ").LastOrDefault();

                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var jwtToken = this.GetJwtTokenValue(token);
                        if (jwtToken != null)
                        {
                            var isValid = await this.AppUserSecretKey(jwtToken);
                            if (isValid)
                            {
                                return await this.AttachUserToContextAsync(jwtToken);
                            }
                        }
                    }
                    catch
                    {
                        return await Task.FromResult(AuthenticateResult.Fail("TokenParseException"));
                    }
                }
            }

            return await Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }

        private async Task<AuthenticateResult> AttachUserToContextAsync(JwtSecurityToken token)
        {
            var userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var roles = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            var username = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            if (userId != null && roles != null && username != null)
            {
                var claims = new[]
                {
                    userId,
                    roles,
                    username,
                };

                var claimsIdentity = new ClaimsIdentity(claims, AppConstants.AppAuhtHeader);
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);
                return await Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return await Task.FromResult(AuthenticateResult.Fail("User does not exist"));
        }

        private async Task<bool> AppUserSecretKey(JwtSecurityToken jwtToken)
        {
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var auroraUserSecretKey = jwtToken.Claims.FirstOrDefault(x => x.Type == AppConstants.AppUserSecretKey);
            if (auroraUserSecretKey != null && userId != null)
            {
                return await this.userService.ValidateAuroraUserSecretKey(userId.Value, auroraUserSecretKey.Value);
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
                return (JwtSecurityToken)validatedToken;
            }
            catch
            {
                return null;
            }
        }
    }
}