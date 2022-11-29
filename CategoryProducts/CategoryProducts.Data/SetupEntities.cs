namespace CategoryProducts.Data
{
    using CategoryProducts.Constraints;
    using CategoryProducts.Data.Models.User;

    using Microsoft.EntityFrameworkCore;

    public static class SetupEntities
    {
        public static void Setup(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(x => x.Roles)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .IsRequired(true);

                entity.Property(x => x.UserName)
                    .HasMaxLength(ModelConstraints.UserNameMaxLength)
                    .IsRequired(true);

                entity.Property(x => x.NormalizedUserName)
                    .HasMaxLength(ModelConstraints.UserNameMaxLength)
                    .IsRequired(true);

                entity.Property(x => x.PhoneNumber)
                    .HasMaxLength(ModelConstraints.PhoneNumberMaxLength);

                entity.Property(x => x.Email)
                    .HasMaxLength(ModelConstraints.EmailMaxLength);

                entity.Property(x => x.NormalizedEmail)
                    .HasMaxLength(ModelConstraints.EmailMaxLength);

                entity.HasMany(x => x.Products)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasMany(x => x.UsersRoles)
                    .WithOne(x => x.Role)
                    .HasForeignKey(x => x.RoleId)
                    .IsRequired();

                entity.Property(x => x.Name)
                    .HasMaxLength(ModelConstraints.RoleNameMaxLength);

                entity.Property(x => x.NormalizedName)
                    .HasMaxLength(ModelConstraints.RoleNameMaxLength);
            });
        }
    }
}