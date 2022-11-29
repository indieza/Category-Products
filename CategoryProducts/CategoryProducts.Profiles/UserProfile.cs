namespace CategoryProducts.Profiles
{
    using AutoMapper;

    using CategoryProducts.Data.Models.User;
    using CategoryProducts.ViewModels.User;

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            this.CreateMap<User, UserViewModel>();
        }
    }
}