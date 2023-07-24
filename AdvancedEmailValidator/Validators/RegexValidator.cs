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
// File: RegexValidator.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Models;

#endregion

namespace AdvancedEmailValidator.Validators;

public class RegexValidator : IRegexValidator
{
    private readonly Regex _regex =
        new(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly Regex _simpleRegex = new("^[^\\s]+@[^\\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public Task<ValidationResult<RegexValidationResult>> IsValidSimpleAsync(string email)
    {
        var match = _simpleRegex.Match(email);

        var result = match.Success
            ? new ValidationResult<RegexValidationResult>
            {
                IsValid = true,
                Message = "Valid Email",
                ValidationDetails = new RegexValidationResult
                {
                    Captures = match.Captures
                }
            }
            : new ValidationResult<RegexValidationResult>
            {
                IsValid = false,
                Message = "Email did not pass regex Validation",
                ValidationDetails = new RegexValidationResult
                {
                    Captures = match.Captures
                }
            };

        return Task.FromResult(result);
    }

    public Task<ValidationResult<RegexValidationResult>> IsValidAsync(string email, Regex customRegex)
    {
        var regex = customRegex ?? _regex;

        var match = regex.Match(email);
        var result = match.Success
            ? new ValidationResult<RegexValidationResult>
            {
                IsValid = true,
                Message = "Valid Email",
                ValidationDetails = new RegexValidationResult
                {
                    Captures = match.Captures
                }
            }
            : new ValidationResult<RegexValidationResult>
            {
                IsValid = false,
                Message = "Email did not pass regex Validation",
                ValidationDetails = new RegexValidationResult
                {
                    Captures = match.Captures
                }
            };

        return Task.FromResult(result);
    }
}