namespace CategoryProducts.InputModels.User
{
    using CategoryProducts.Constraints;
    using CategoryProducts.CustomAttributes;
    using CategoryProducts.Resources;

    using System;
    using System.ComponentModel.DataAnnotations;

    public class LoginInputModel
    {
        public LoginInputModel()
        {
            this.RememberMe = true;
            this.ExpireDate = DateTime.MaxValue;
            this.ShowPassword = false;
        }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [TextLength(ModelConstraints.UserNameMinLength, ModelConstraints.UserNameMaxLength, false, ErrorMessageResourceName = "InvalidTextLength", ErrorMessageResourceType = typeof(Resource))]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessageResourceName = "InvalidUsernameCharacters", ErrorMessageResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public DateTime ExpireDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public bool RememberMe { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public bool ShowPassword { get; set; }
    }
}