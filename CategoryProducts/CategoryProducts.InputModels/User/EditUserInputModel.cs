namespace CategoryProducts.InputModels.User
{
    using System.ComponentModel.DataAnnotations;

    using CategoryProducts.Constraints;

    public class EditUserInputModel
    {
        [StringLength(ModelConstraints.NameMaxLength, MinimumLength = ModelConstraints.NameMinLength)]
        public string? FirstName { get; set; }

        [StringLength(ModelConstraints.NameMaxLength, MinimumLength = ModelConstraints.NameMinLength)]
        public string? LastName { get; set; }
    }
}