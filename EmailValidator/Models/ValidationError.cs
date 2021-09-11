namespace EmailValidator.Models
{
    using System;

    public class ValidationError : ValidationResult
    {
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
}