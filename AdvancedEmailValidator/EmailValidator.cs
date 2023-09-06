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
// Project: Advanced Email Validator
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
    private readonly IDnsValidator _dnsValidator;
    private readonly ITypoCheck _typoCheck;
    private readonly IRegexValidator _regexValidator;
    private readonly IDisposableValidator _disposableValidator;
    private readonly IBuildDependencies _buildDependencies;

    public EmailValidator(IDnsValidator dnsValidator, ITypoCheck typoCheck, IRegexValidator regexValidator, IDisposableValidator disposableValidator, IBuildDependencies buildDependencies)
    {
        _dnsValidator = dnsValidator;
        _typoCheck = typoCheck;
        _regexValidator = regexValidator;
        _disposableValidator = disposableValidator;
        _buildDependencies = buildDependencies;
    }

    public async Task<EmailValidationResult> ValidateAsync(string email, ValidationOptions options = null)
    {
        var validationResult = new EmailValidationResult();

        options ??= new ValidationOptions
        {
            IsStrict = false,
            ValidateDisposable = true,
            ValidateMx = true,
            ValidateRegex = true,
            ValidateSimpleRegex = false,
            ValidateTypo = true
        };

        if (options.ValidateSimpleRegex)
        {
            validationResult.SimpleRegexResult = await _regexValidator.IsValidSimpleAsync(email);
        }

        if (options.ValidateRegex)
        {
            validationResult.StandardRegexResult = await _regexValidator.IsValidAsync(email, options.CustomRegex);
        }

        if (options.ValidateMx)
        {
            validationResult.MxResult = await _dnsValidator.QueryAsync(email, options.IsStrict);
        }

        if (options.ValidateDisposable)
        {
            await _buildDependencies.CheckDependencies();

            validationResult.DisposableResult = await _disposableValidator.ValidateAsync(email);
        }

        if (options.ValidateTypo)
        {
            validationResult.TypoResult = await _typoCheck.SuggestAsync(email, options.TypoOptions);
        }

        return validationResult;
    }
}