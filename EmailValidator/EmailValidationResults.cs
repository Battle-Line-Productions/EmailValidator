namespace EmailValidator
{
    using System.Collections.Generic;

    public class EmailValidationResults
    {
        /// <summary>
        /// True if all performed checks are valid, false if any of the performed checks is not valid
        /// </summary>
        public bool IsValid { get; set; }

        public List<ValidationResult> ValidationResults { get; set; }
    }
}