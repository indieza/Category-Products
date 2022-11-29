namespace CategoryProducts.Data
{
    using CategoryProducts.Data.Models.Enums;
    using CategoryProducts.Data.Models.User;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;

    public static class InsertSystemData
    {
        private static readonly List<Role> Roles = new()
        {
            new Role()
            {
                Id = "d7d72ec2-dfdd-4dc9-b56e-c38b43d3b377",
                Name = RoleType.Administrator.ToString(),
                NormalizedName = RoleType.Administrator.ToString().ToUpper(),
                Level = (int)RoleType.Administrator,
                ConcurrencyStamp = "4980ca05-511f-45e5-a4af-ee8ffee6b6c5",
            },
            new Role()
            {
                Id = "caffa87b-e581-4b56-859e-de9add2789df",
                Name = RoleType.User.ToString(),
                NormalizedName = RoleType.User.ToString().ToUpper(),
                Level = (int)RoleType.User,
                ConcurrencyStamp = "269ed3a5-a269-4ff0-8b58-a3050b6c5f8b",
            },
        };

        public static void Insert(ModelBuilder modelBuilder)
        {
            foreach (var item in Roles)
            {
                modelBuilder.Entity<Role>().HasData(item);
            }
        }
    }
}