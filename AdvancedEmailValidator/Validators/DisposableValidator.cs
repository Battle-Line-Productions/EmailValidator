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
// File: DisposableValidator.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System;
using System.IO;
using System.Threading.Tasks;
using AdvancedEmailValidator.Extensions;
using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Models;

#endregion

namespace AdvancedEmailValidator.Validators;

public class DisposableValidator : IDisposableValidator
{
    private readonly IFileReader _fileReader;
    private readonly string _disposableEmailFile = $"{Path.GetTempPath()}disposable_email_blocklist.conf";


    public DisposableValidator(IFileReader fileReader)
    {
        _fileReader = fileReader;

        if (!_fileReader.Exists(_disposableEmailFile))
        {
            throw new FileNotFoundException(nameof(_disposableEmailFile));
        }
    }

    public async Task<ValidationResult<DisposableValidationResult>> ValidateAsync(string email)
    {
        var disposableEmailListing = await _fileReader.ReadAllLinesAsync(_disposableEmailFile);

        if (Array.Exists(disposableEmailListing, line => line.Equals(email.GetEmailDomain(), StringComparison.OrdinalIgnoreCase)))
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