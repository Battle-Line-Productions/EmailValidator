namespace EmailValidator.Models
{
    using System.Text.RegularExpressions;

    public class RegexValidationResult : ValidationResult
    {
        public CaptureCollection Captures { get; set; }
    }
}