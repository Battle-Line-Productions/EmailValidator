namespace EmailValidator.Validators
{
    using System.Collections.Generic;
    using System.IO;
    using Extensions;
    using Models;

    public static class DisposableValidator
    {
        private static readonly List<string> DisposableEmailListing = new();
        private static readonly string DisposableEmailFile = $"{Path.GetTempPath()}disposable_email_blocklist.conf";

        static DisposableValidator()
        {
            if (!File.Exists(DisposableEmailFile))
            {
                throw new FileNotFoundException(nameof(DisposableEmailFile));
            }

            DisposableEmailListing.AddRange(File.ReadLines(DisposableEmailFile));
        }

        public static ValidationResult<DisposableValidationResult> Validate(string email)
        {
            if (DisposableEmailListing.Contains(email.GetEmailDomain()))
            {

                return new ValidationResult<DisposableValidationResult>
                {
                    Message = "Email is on the list of disposable email domains",
                    IsValid = false
                };
            }
            
            return new ValidationResult<DisposableValidationResult>
            {
                Message = "Email is not a disposable domain",
                IsValid = true
            };
        }
    }
}
