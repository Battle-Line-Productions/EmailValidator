namespace EmailValidator.Models
{
    public class TypoValidationResult : ValidationResult
    {
        public string Address { get; set; }
        public string Domain { get; set; }
        public string SuggestedEmail { get; set; }
        public string OriginalEmail { get; set; }
    }
}