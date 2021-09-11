namespace EmailValidator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Validators;

    public class EmailValidator
    {
        private readonly ValidationOptions _options;

        public EmailValidator()
        {
            _options = new ValidationOptions();
        }

        public EmailValidator(ValidationOptions options)
        {
            _options = options;
        }

        public EmailValidationResults Validate(string email)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (_options.ValidateSimpleRegex)
            {
                results.Add(RegexValidator.IsValidSimple(email));
            }

            if (_options.ValidateRegex)
            {
                results.Add(RegexValidator.IsValid(email, _options));
            }

            if (_options.ValidateMx)
            {
                var dnsValidator = new DnsValidator(null, _options);
                results.Add(dnsValidator.Query(email));
            }

            return new EmailValidationResults
            {
                IsValid = results.All(x => x.IsValid),
                ValidationResults = results
            };
        }
    }
}