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
// File: ValidationOptions.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System.Text.RegularExpressions;

#endregion

namespace AdvancedEmailValidator.Models;

public class ValidationOptions
{
    /// <summary>
    ///     Validates email against a standard regex string recommended by ISO
    /// </summary>
    public bool ValidateRegex { get; set; } = true;

    /// <summary>
    ///     Validates email against a simple regex string
    ///     Checks for @ sign and dot after at sign
    /// </summary>
    public bool ValidateSimpleRegex { get; set; } = true;

    /// <summary>
    ///     Will Validate the email being checked has a valid mx record on the domain
    /// </summary>
    public bool ValidateMx { get; set; } = true;

    /// <summary>
    ///     Validates the email provided for common typos based on ___ algorithm
    /// </summary>
    public bool ValidateTypo { get; set; } = true;

    /// <summary>
    ///     Validates the email domain against a common kept list of disposable email domains
    /// </summary>
    public bool ValidateDisposable { get; set; } = true;

    /// <summary>
    ///     Input regex for email validation
    /// </summary>
    public Regex CustomRegex { get; set; }

    /// <summary>
    ///     If IsStrict is set to true then false will be returned in situations where email might still send successfully but
    ///     isn't recommended.
    ///     Example: MX record is missing but A record exists would return false for IsValid even though spec says email might
    ///     still send.
    /// </summary>
    public bool IsStrict { get; set; } = true;

    /// <summary>
    ///     A set of options used to determine how the typo functionality works
    /// </summary>
    public TypoOptions TypoOptions { get; set; } = new();
}