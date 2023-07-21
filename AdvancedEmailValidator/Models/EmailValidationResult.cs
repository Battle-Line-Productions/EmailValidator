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
// File: ValidationResult.cs
// ---------------------------------------------------------------------------

#endregion

namespace AdvancedEmailValidator.Models;

public class ValidationResult<T>
{
    public bool IsValid { get; set; }

    public string Message { get; set; }

    public T ValidationDetails { get; set; }
}

public class EmailValidationResult
{
    public ValidationResult<RegexValidationResult> SimpleRegexResult { get; set; }
    public ValidationResult<RegexValidationResult> StandardRegexResult { get; set; }
    public ValidationResult<DnsValidationResult> MxResult { get; set; }
    public ValidationResult<DisposableValidationResult> DisposableResult { get; set; }
    public ValidationResult<TypoValidationResult> TypoResult { get; set; }
}