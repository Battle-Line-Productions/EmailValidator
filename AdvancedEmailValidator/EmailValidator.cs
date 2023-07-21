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
// File: EmailValidator.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Models;
using System.Threading.Tasks;

#endregion

namespace AdvancedEmailValidator;

public class EmailValidator : IEmailValidator
{
    private readonly ValidationOptions _options;
    private readonly IDnsValidator _dnsValidator;
    private readonly ITypoCheck _typoCheck;
    private readonly IRegexValidator _regexValidator;
    private readonly IDisposableValidator _disposableValidator;

    public EmailValidator(IDnsValidator dnsValidator, ITypoCheck typoCheck, IRegexValidator regexValidator, IDisposableValidator disposableValidator, IBuildDependencies buildDependencies)
    {
        _options = new ValidationOptions();
        _dnsValidator = dnsValidator;
        _typoCheck = typoCheck;
        _regexValidator = regexValidator;
        _disposableValidator = disposableValidator;

        buildDependencies.CheckDependencies().GetAwaiter().GetResult();
    }

    public EmailValidator(ValidationOptions options, IDnsValidator dnsValidator, ITypoCheck typoCheck, IRegexValidator regexValidator, IDisposableValidator disposableValidator, IBuildDependencies buildDependencies)
    {
        _options = options;
        _dnsValidator = dnsValidator;
        _typoCheck = typoCheck;
        _regexValidator = regexValidator;
        _disposableValidator = disposableValidator;

        buildDependencies.CheckDependencies().GetAwaiter().GetResult();
    }

    public async Task<EmailValidationResult> ValidateAsync(string email)
    {
        var validationResult = new EmailValidationResult();

        if (_options.ValidateSimpleRegex)
        {
            validationResult.SimpleRegexResult = await _regexValidator.IsValidSimpleAsync(email);
        }

        if (_options.ValidateRegex)
        {
            validationResult.StandardRegexResult = await _regexValidator.IsValidAsync(email, _options.CustomRegex);
        }

        if (_options.ValidateMx)
        {
            validationResult.MxResult = await _dnsValidator.QueryAsync(email);
        }

        if (_options.ValidateDisposable)
        {
            validationResult.DisposableResult = await _disposableValidator.ValidateAsync(email);
        }

        if (_options.ValidateTypo)
        {
            validationResult.TypoResult = await _typoCheck.SuggestAsync(email);
        }

        return validationResult;
    }
}