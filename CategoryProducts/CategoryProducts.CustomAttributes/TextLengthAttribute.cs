using System.ComponentModel.DataAnnotations;

namespace CategoryProducts.CustomAttributes
{
    public class TextLengthAttribute : ValidationAttribute
    {
        private readonly int minLength;
        private readonly int maxLength;
        private readonly bool allowEmpty;

        public TextLengthAttribute(int minLength, int maxLength, bool allowEmpty = false)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
            this.allowEmpty = allowEmpty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, this.minLength, this.maxLength);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var text = value?.ToString();

            if (text == null && !this.allowEmpty)
            {
                return new ValidationResult(this.ErrorMessage);
            }
            else if (text == null && this.allowEmpty)
            {
                return ValidationResult.Success;
            }
            else if (text.Trim().Length >= this.minLength && text.Trim().Length <= this.maxLength)
            {
                return ValidationResult.Success;
            }
            else if (text.Trim().Length == 0 && this.allowEmpty)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(this.ErrorMessage);
        }
    }
}