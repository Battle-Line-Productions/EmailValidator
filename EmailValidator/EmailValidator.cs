namespace EmailValidator
{
    using System;
    using Models;

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
            throw new NotImplementedException();
        }
    }
}