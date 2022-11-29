namespace CategoryProducts.Profiles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using CategoryProducts.Data.Models.Shop;
    using CategoryProducts.ViewModels.Shop;

    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public class ShopProfile : Profile
    {
        public ShopProfile()
        {
            this.CreateMap<Category, CategoryViewModel>();
            this.CreateMap<Product, ProductViewModel>();
        }
    }
}