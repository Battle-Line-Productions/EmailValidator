namespace EmailValidator.Validators
{
    using System.Text.RegularExpressions;
    using Models;

    public static class RegexValidator
    {
        private static readonly Regex SimpleRegex = new("^[^\\s]+@[^\\s]+$", RegexOptions.IgnoreCase);

        private static readonly Regex Regex =
            new("^[^-.;<>'\"\\s]+(\\.[^-.;<>'\"\\s]+)*@[^-.;<>'\"\\s]+(\\.[^-.;<>'\"\\s]+)*$", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Performs a very simple regex validation on email strings checking for the following
        /// 1. At least one @ Sign
        /// 2. At least one non-whitespace character before the @ sign
        /// 3. At least one non-whitespace character after the @ sign
        /// </summary>
        /// <param name="email">the string to validate as an email</param>
        /// <returns><see cref="ValidationResult"/> containing a validation message and a boolean isValid</returns>
        public static ValidationResult IsValidSimple(string email)
        {
            var match = SimpleRegex.Match(email);
            return match.Success
                ? new RegexValidationResult { IsValid = true, Message = "Valid Email", Captures = match.Captures }
                : new RegexValidationResult
                    { IsValid = false, Message = "Email did not pass regex Validation", Captures = match.Captures };
        }

        /// <summary>
        /// Performs a regex validation using standard regex with the rules below or a custom regex defined by consumer
        /// 1. At least one @ Sign
        /// 2. string before the @ sign does not begin with a period, string after the @ sign does not begin with a period
        /// 3. string before the @ sign will never contain two periods in a row, string after the @ sign will never contain two periods in a row
        /// 4. No whitespace characters are contains in the section before or after the @ sign
        /// 5. The email does not contain symbols ; < > ' "
        /// </summary>
        /// <param name="email">the string to validate as an email</param>
        /// <param name="options">The options injected by the consumer to define how email validation should occur</param>
        /// <returns><see cref="ValidationResult"/> containing a validation message and a boolean isValid</returns>
        public static ValidationResult IsValid(string email, ValidationOptions options)
        {
            var regex = options.CustomRegex ?? Regex;
            
            var match = regex.Match(email);
            return match.Success
                ? new RegexValidationResult { IsValid = true, Message = "Valid Email", Captures = match.Captures }
                : new RegexValidationResult
                    { IsValid = false, Message = "Email did not pass regex Validation", Captures = match.Captures };
        }
    }
}
