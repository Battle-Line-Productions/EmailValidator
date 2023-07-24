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
// File: TypoOptions.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace AdvancedEmailValidator.Models;

public class TypoOptions
{
    /// <summary>
    ///     A list of domains to check typo's of
    /// </summary>
    public List<string> Domains { get; set; } = new();

    /// <summary>
    ///     A list of second level domains to check typos of
    /// </summary>
    public List<string> SecondLevelDomains { get; set; } = new();

    /// <summary>
    ///     A list of top level domains to check typo's of
    /// </summary>
    public List<string> TopLevelDomains { get; set; } = new();

    public int? DomainThreshold { get; set; }

    public int? SecondLevelThreshold { get; set; }

    public int? TopLevelThreshold { get; set; }
}