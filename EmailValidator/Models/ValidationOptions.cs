namespace EmailValidator.Models
{
    using System.Text.RegularExpressions;

    public class ValidationOptions
    {
        /// <summary>
        /// Validates email against a standard regex string recomended by ISO 
        /// </summary>
        public bool ValidateRegex { get; set; } = true;
        
        /// <summary>
        /// Validates email against a simple regex string
        /// Checks for @ sign and dot after at sign
        /// </summary>
        public bool ValidateSimpleRegex { get; set; } = false;
        
        /// <summary>
        /// Will Validate the email being checked has a valid mx record on the domain
        /// </summary>
        public bool ValidateMx { get; set; } = true;
        
        /// <summary>
        /// Checks the SMTP version of the domain to see if the email is valid
        /// This poses a potential security risk and most SMTP servers block it for this reason
        /// </summary>
        public bool ValidateSmtp { get; set; } = false;
        
        /// <summary>
        /// Validates the email provided for common typos based on ___ algorithm
        /// </summary>
        public bool ValidateTypo { get; set; } = true;
        
        /// <summary>
        /// Validates the email domain against a common kept list of disposable email domains
        /// </summary>
        public bool ValidateDisposable { get; set; } = true;

        /// <summary>
        /// Input regex for email validation
        /// </summary>
        public Regex CustomRegex { get; set; }
    }
}