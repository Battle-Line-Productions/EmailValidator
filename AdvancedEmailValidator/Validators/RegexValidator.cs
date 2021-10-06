namespace AdvancedEmailValidator.Validators
{
    using System.Text.RegularExpressions;
    using Models;

    public static class RegexValidator
    {
        private static readonly Regex SimpleRegex = new("^[^\\s]+@[^\\s]+$", RegexOptions.IgnoreCase);

        private static readonly Regex Regex =
            new(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Performs a very simple regex validation on email strings checking for the following
        /// 1. At least one @ Sign
        /// 2. At least one non-whitespace character before the @ sign
        /// 3. At least one non-whitespace character after the @ sign
        /// </summary>
        /// <param name="email">the string to validate as an email</param>
        /// <returns><see cref="ValidationResult&lt;RegexValidationResult&gt;"/> containing a validation message and a boolean isValid</returns>
        public static ValidationResult<RegexValidationResult> IsValidSimple(string email)
        {
            var match = SimpleRegex.Match(email);


            return match.Success
                ? new ValidationResult<RegexValidationResult>
                {
                    IsValid = true,
                    Message = "Valid Email",
                    ValidationDetails = new RegexValidationResult
                    {
                        Captures = match.Captures
                    }
                }
                : new ValidationResult<RegexValidationResult>
                {
                    IsValid = false,
                    Message = "Email did not pass regex Validation",
                    ValidationDetails = new RegexValidationResult
                    {
                        Captures = match.Captures
                    }
                };
        }

        /// <summary>
        /// Performs a regex validation or a custom regex defined by consumer
        /// </summary>
        /// <param name="email">the string to validate as an email</param>
        /// <param name="customRegex">The the custom regex option injected by the consumer to define how email validation should occur</param>
        /// <returns><see cref="ValidationResult&lt;RegexValidationResult&gt;"/> containing a validation message and a boolean isValid</returns>
        public static ValidationResult<RegexValidationResult> IsValid(string email, Regex customRegex = null)
        {
            var regex = customRegex ?? Regex;
            
            var match = regex.Match(email);
            return match.Success
                ? new ValidationResult<RegexValidationResult>
                {
                    IsValid = true,
                    Message = "Valid Email",
                    ValidationDetails = new RegexValidationResult
                    {
                        Captures = match.Captures
                    }
                }
                : new ValidationResult<RegexValidationResult>
                {
                    IsValid = false,
                    Message = "Email did not pass regex Validation",
                    ValidationDetails = new RegexValidationResult
                    {
                        Captures = match.Captures
                    }
                };
        }
    }
}
