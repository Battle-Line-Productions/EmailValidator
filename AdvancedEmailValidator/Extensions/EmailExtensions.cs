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
// File: EmailExtensions.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System;
using System.Linq;

#endregion

namespace AdvancedEmailValidator.Extensions;

public static class EmailExtensions
{
    public static string GetEmailDomain(this string email)
    {
        var parts = email.Split("@", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            throw new ArgumentException($"{email} is not a valid email");
        }

        var domainParts = parts[1].Split(".", StringSplitOptions.RemoveEmptyEntries);
        if (domainParts.Length < 2)
        {
            throw new ArgumentException($"The domain in {email} is not valid");
        }

        return parts[1];
    }

    public static (string topLevelDomain, string secondLevelDomain, string domain, string emailsParts, string fullAddress) SplitEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentNullException(nameof(email));
        }

        email = email.Trim();

        var emailParts = email.Split("@");

        if (emailParts.Length != 2 || email.Any(x => string.IsNullOrWhiteSpace(x.ToString())) || emailParts[1].StartsWith("."))
        {
            throw new ArgumentException($"{email} is not a valid email");
        }

        var domain = emailParts.Last();
        var domainParts = domain.Split(".");
        var secondLevelDomain = string.Empty;
        var topLevelDomain = string.Empty;

        if (domainParts.Length < 2)
        {
            throw new ArgumentException("Email is missing a top level domain");
        }

        secondLevelDomain = domainParts[0];
        topLevelDomain = domainParts.Last();

        var fullAddress = string.Join("@", emailParts);

        return (topLevelDomain, secondLevelDomain, domain, emailParts[0], fullAddress);
    }

    public static string EncodeEmail(this string email)
    {
        // http://en.wikipedia.org/wiki/Email_address#Syntax
        var result = Uri.EscapeDataString(email);

        return result.Replace("%20", "")
            .Replace("%25", "%")
            .Replace("%5E", "^")
            .Replace("%60", "`")
            .Replace("%7B", "{")
            .Replace("%7C", "|")
            .Replace("%7D", "}");
    }
}