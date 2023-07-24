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
// File: TypoValidationResult.cs
// ---------------------------------------------------------------------------

#endregion

namespace AdvancedEmailValidator.Models;

public class TypoValidationResult
{
    public string Address { get; set; }
    public string Domain { get; set; }
    public string SuggestedEmail { get; set; }
    public string OriginalEmail { get; set; }
}