namespace CategoryProducts.ViewModels.System
{
    public class JwtOptions
    {
        public string SecurityKey { get; set; }

        public string ValidIssuer { get; set; }

        public string ValidAudience { get; set; }
    }
}