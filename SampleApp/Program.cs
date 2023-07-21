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
using AdvancedEmailValidator;
using AdvancedEmailValidator.Models;

#endregion

namespace SampleApp;

internal static class Program
{
    private static void Main(string[] args)
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

        //var validator = new EmailValidator(options);

        //var result = validator.Validate("funkel1989@gmail.com");

        //Console.WriteLine(result.ToString());
        Console.ReadLine();
    }
}