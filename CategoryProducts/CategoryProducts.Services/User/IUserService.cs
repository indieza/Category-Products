namespace CategoryProducts.Services.User
{
    using CategoryProducts.Data.Models.User;
    using CategoryProducts.InputModels.User;
    using CategoryProducts.ViewModels.System;
    using CategoryProducts.ViewModels.User;

    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<User> GetUserById(string userId);

        Task<bool> ValidateAuroraUserSecretKey(string userId, string auroraUserSecretKey);

        Task<CompletedOperation<LoginInputModel?>> RegisterAsync(RegisterInputModel model);

        Task<CompletedOperation<string?>> LoginAsync(LoginInputModel model);

        Task<CompletedOperation<UserViewModel?>> GetUserProfile(string username);

        Task<CompletedOperation<UserViewModel?>> EditUserProfile(EditUserInputModel model, string username);
    }
}