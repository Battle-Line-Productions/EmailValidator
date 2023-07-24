#region Copyright

// ---------------------------------------------------------------------------
// Copyright (c) 2023 BattleLine Productions LLC. All rights reserved.
// 
// Licensed under the BattleLine Productions LLC license agreement.
// See LICENSE file in the project root for full license information.
// 
// Author: Michael Cavanaugh
// Company: BattleLine Productions LLC
// Date: 07/20/2023
// Project: Frontline CRM
// File: Program.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System;
using System.Net.Http;
using System.Threading.Tasks;
using AdvancedEmailValidator;
using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Models;
using AdvancedEmailValidator.Validators;
using Microsoft.Extensions.Http;

#endregion

namespace SampleApp;

public class SimpleHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return new HttpClient();
    }
}

internal static class Program
{
    private static async Task Main(string[] args)
    {
        IBuildDependencies buildDependencies = new BuildDependencies(new SimpleHttpClientFactory());
        buildDependencies.CheckDependencies().GetAwaiter().GetResult();

        var validationOptions = new ValidationOptions();

        IDnsValidator dnsValidator = new DnsValidator(validationOptions);
        ITypoCheck typoCheck = new TypoCheck(validationOptions.TypoOptions);
        IRegexValidator regexValidator = new RegexValidator();
        IFileReader fileReader = new FileReader();
        IDisposableValidator disposableValidator = new DisposableValidator(fileReader);
        
        var emailValidator = new EmailValidator(dnsValidator, typoCheck, regexValidator, disposableValidator, buildDependencies);

        string emailToValidate;
        string userInput;

        do
        {
            Console.WriteLine("Please enter an email to validate:");
            emailToValidate = Console.ReadLine();

            var validationResult = await emailValidator.ValidateAsync(emailToValidate);

            Console.WriteLine($"Validation result for email '{emailToValidate}':");
            Console.WriteLine("Simple Regex Result: " + (validationResult.SimpleRegexResult?.IsValid ?? false));
            Console.WriteLine("Standard Regex Result: " + (validationResult.StandardRegexResult?.IsValid ?? false));

            Console.WriteLine("Do you want to validate another email? Press '1' to validate another email, '2' to exit.");
            userInput = Console.ReadLine();

        } while (userInput == "1");
    }

}