﻿#region Copyright

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
// File: ValidationType.cs
// ---------------------------------------------------------------------------

#endregion

namespace AdvancedEmailValidator.Models;

public enum ValidationType
{
    RegexValidation,
    TypoValidation,
    DisposableValidation,
    MxValidation
}