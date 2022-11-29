namespace CategoryProducts.Services.User
{
    using AutoMapper;

    using CategoryProducts.Constraints;
    using CategoryProducts.Data;
    using CategoryProducts.Data.Models.Enums;
    using CategoryProducts.Data.Models.User;
    using CategoryProducts.InputModels.User;
    using CategoryProducts.Resources;
    using CategoryProducts.ViewModels.System;
    using CategoryProducts.ViewModels.User;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IOptions<JwtOptions> configuration;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<Resource> localizare;
        private readonly Context db;

        public UserService(UserManager<User> userManager, IOptions<JwtOptions> configuration, IMapper mapper, Context db, IStringLocalizer<Resource> localizare)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.mapper = mapper;
            this.localizare = localizare;
            this.db = db;
        }

        public async Task<User> GetUserById(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<CompletedOperation<string?>> LoginAsync(LoginInputModel model)
        {
            var user = await this.userManager.FindByNameAsync(model.UserName);
            if (user == null || !await this.userManager.CheckPasswordAsync(user, model.Password))
            {
                return new CompletedOperation<string?>()
                {
                    Key = "Error",
                    Title = this.localizare["Error"],
                    Message = this.localizare["User does not exist"],
                    Response = null,
                };
            }

            var signingCredentials = this.GetSigningCredentials();
            var claims = await this.GetClaims(user);
            var tokenOptions = this.GenerateTokenOptions(signingCredentials, claims, model.ExpireDate);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new CompletedOperation<string?>()
            {
                Key = "Success",
                Title = this.localizare["Success"],
                Message = this.localizare["Successfully login"],
                Response = token,
            };
        }

        public async Task<CompletedOperation<LoginInputModel?>> RegisterAsync(RegisterInputModel model)
        {
            var user = new User() { UserName = model.UserName };
            var result = await this.userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new CompletedOperation<LoginInputModel?>()
                {
                    Key = "Error",
                    Title = this.localizare["Error"],
                    Message = string.Join("\n", result.Errors.Select(x => $"{x.Code}: {x.Description}")),
                    Response = null,
                };
            }

            await this.userManager.AddToRoleAsync(user, nameof(RoleType.User));
            return new CompletedOperation<LoginInputModel?>()
            {
                Key = "Success",
                Title = this.localizare["Error"],
                Message = this.localizare["Successfully create an user account"],
                Response = new LoginInputModel()
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    RememberMe = model.RememberMe,
                    ExpireDate = model.ExpireDate,
                    ShowPassword = model.ShowPassword,
                },
            };
        }

        public async Task<bool> ValidateAuroraUserSecretKey(string userId, string auroraUserSecretKey)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return user.AppUserSecretKey == auroraUserSecretKey;
            }

            return false;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(this.configuration.Value.SecurityKey);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(AppConstants.AppUserSecretKey, user.AppUserSecretKey),
            };

            var roles = await this.userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims, DateTime expireDate)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: this.configuration.Value.ValidIssuer,
                audience: this.configuration.Value.ValidAudience,
                claims: claims,
                expires: expireDate,
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        public async Task<CompletedOperation<UserViewModel?>> GetUserProfile(string username)
        {
            var target = await this.userManager.FindByNameAsync(username);
            if (target != null)
            {
                return new CompletedOperation<UserViewModel?>()
                {
                    Key = "Success",
                    Title = this.localizare["Success"],
                    Message = this.localizare["User exist"],
                    Response = this.mapper.Map<UserViewModel>(target),
                };
            }

            return new CompletedOperation<UserViewModel?>()
            {
                Key = "Error",
                Title = this.localizare["Error"],
                Message = this.localizare["User does not exist"],
                Response = null,
            };
        }

        public async Task<CompletedOperation<UserViewModel?>> EditUserProfile(EditUserInputModel model, string username)
        {
            var target = await this.userManager.FindByNameAsync(username);
            if (target != null)
            {
                target.FirstName = model.FirstName;
                target.LastName = model.LastName;
                this.db.Users.Update(target);
                await this.db.SaveChangesAsync();

                return new CompletedOperation<UserViewModel?>()
                {
                    Key = "Success",
                    Title = this.localizare["Success"],
                    Message = this.localizare["Successfully update user profile"],
                    Response = this.mapper.Map<UserViewModel>(target),
                };
            }

            return new CompletedOperation<UserViewModel?>()
            {
                Key = "Error",
                Title = this.localizare["Error"],
                Message = this.localizare["User does not exist"],
                Response = new UserViewModel(),
            };
        }
    }
}