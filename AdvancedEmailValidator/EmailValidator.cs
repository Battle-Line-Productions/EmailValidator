namespace AdvancedEmailValidator
{
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

        public (ValidationResult<RegexValidationResult>, 
            ValidationResult<RegexValidationResult>,
            ValidationResult<DnsValidationResult>, 
            ValidationResult<DisposableValidationResult>, 
            ValidationResult<TypoValidationResult>) Validate(string email)
        {
            var dnsValidator = new DnsValidator(null, _options);
            var typo = new TypoCheck(_options.TypoOptions);

            var simpleRegexResult = _options.ValidateSimpleRegex ? RegexValidator.IsValidSimple(email) : null;
            var standardRegexResult = _options.ValidateRegex ? RegexValidator.IsValid(email, _options.CustomRegex) : null;
            var mxResult = _options.ValidateMx ? dnsValidator.Query(email) : null;
            var disposableResult = _options.ValidateDisposable ? DisposableValidator.Validate(email) : null;
            var typoResult = _options.ValidateTypo ? typo.Suggest(email) : null;

            return (simpleRegexResult, standardRegexResult, mxResult, disposableResult, typoResult);
        }
    }
}