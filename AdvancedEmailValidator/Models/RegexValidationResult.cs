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
// File: RegexValidationResult.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

#endregion

namespace AdvancedEmailValidator.Models;

public class RegexValidationResult
{
    [JsonIgnore] // We are ignoring this property during serialization
    public CaptureCollection Captures { get; set; }

    public List<string> SerializableCaptures
    {
        get
        {
            var serializableCaptures = new List<string>();

            if (Captures != null)
            {
                serializableCaptures.AddRange(Captures.Select(capture => capture.Value));
            }

            return serializableCaptures;
        }
        
    }
}
