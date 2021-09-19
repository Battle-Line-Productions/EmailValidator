namespace EmailValidator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Validators;

    public class EmailValidator
    {
        private readonly ValidationOptions _options;

        public EmailValidator()
        {
            _options = new ValidationOptions();
            var dependencies = new BuildDependencies();
            Task.Run(() => dependencies.CheckDependencies()).Wait();
        }

        public EmailValidator(ValidationOptions options)
        {
            _options = options;
            var dependencies = new BuildDependencies();
            Task.Run(() => dependencies.CheckDependencies()).Wait(); 
        }

        public EmailValidationResults Validate(string email)
        {
            var results = new List<ValidationResult>();

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

            if (_options.ValidateDisposable)
            {
                results.Add(DisposableValidator.Validate(email));
            }

            if (_options.ValidateTypo)
            {
                var typo = new TypoCheck(_options.TypoOptions);
                results.Add(typo.Suggest(email));
            }

            return new EmailValidationResults
            {
                IsValid = results.All(x => x.IsValid),
                ValidationResults = results
            };
        }
    }
}