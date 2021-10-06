namespace AdvancedEmailValidator.Models
{
    public class ValidationResult<T>
    {
        public bool IsValid { get; set; }

        public string Message { get; set; }

        public T ValidationDetails { get; set; }
    }
}