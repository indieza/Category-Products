namespace CategoryProducts.Constraints
{
    public class ModelConstraints
    {
        public const string UserNameRegexPattern = "[a-zA-Z0-9-_.]+";

        public const int NameMinLength = 2;

        public const int NameMaxLength = 60;

        public const int RoleMinLevel = 1;

        public const int RoleMaxLevel = 2;

        public const int UserNameMaxLength = 25;

        public const int UserNameMinLength = 4;

        public const int PhoneNumberMaxLength = 25;

        public const int EmailMaxLength = 45;

        public const int RoleNameMaxLength = 25;

        public const int PasswordMinLength = 6;
    }
}