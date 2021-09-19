namespace SampleApp
{
    using System;
    using EmailValidator;
    using EmailValidator.Models;

    static class Program
    {
        static void Main(string[] args)
        {
            var options = new ValidationOptions
            {
                ValidateSimpleRegex = true,
                IsStrict = true,
                ValidateRegex = true,
                ValidateMx = true,
                ValidateDisposable = true,
                ValidateTypo = true
            };
            
            var validator = new EmailValidator(options);

            var result = validator.Validate("funkel1989@gmail.com");

            Console.WriteLine(result.ToString());
            Console.ReadLine();
        }
    }
}
