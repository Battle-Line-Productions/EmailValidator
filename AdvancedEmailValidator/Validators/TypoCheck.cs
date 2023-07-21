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
// File: TypoCheck.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedEmailValidator.Extensions;
using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Models;

#endregion

namespace AdvancedEmailValidator.Validators;

public class TypoCheck : ITypoCheck
{
    private readonly List<string> _domains = new()
    {
        "msn.com", "bellsouth.net", "telus.net", "comcast.net", "optusnet.com.au", "earthlink.net", "qq.com",
        "sky.com", "icloud.com", "mac.com", "sympatico.ca", "googlemail.com", "att.net", "xtra.co.nz", "web.de",
        "cox.net", "gmail.com", "ymail.com", "aim.com", "rogers.com", "verizon.net", "rocketmail.com", "google.com",
        "optonline.net", "sbcglobal.net", "aol.com", "me.com", "btinternet.com", "charter.net", "shaw.ca"
    };

    private readonly int _domainThreshold = 2;

    private readonly List<string> _secondLevelDomains =
        new() { "yahoo", "hotmail", "mail", "live", "outlook", "gmx" };

    private readonly int _secondLevelThreshold = 2;

    private readonly List<string> _topLevelDomains = new()
    {
        "com", "com.au", "com.tw", "ca", "co.nz", "co.uk", "de", "fr", "it", "ru", "net", "org", "edu", "gov", "jp",
        "nl", "kr", "se", "eu", "ie", "co.il", "us", "at", "be", "dk", "hk", "es", "gr", "ch", "no", "cz", "in",
        "net", "net.au", "info", "biz", "mil", "co.jp", "sg", "hu", "uk"
    };

    private readonly int _topLevelThreshold = 2;

    public TypoCheck(TypoOptions options)
    {
        _domains ??= options?.Domains ?? _domains;
        _secondLevelDomains ??= options?.SecondLevelDomains ?? _secondLevelDomains;
        _topLevelDomains ??= options?.TopLevelDomains ?? _topLevelDomains;
        _domainThreshold = options?.DomainThreshold ?? _domainThreshold;
        _secondLevelThreshold = options?.SecondLevelThreshold ?? _secondLevelThreshold;
        _topLevelThreshold = options?.TopLevelThreshold ?? _topLevelThreshold;
    }

    public Task<ValidationResult<TypoValidationResult>> SuggestAsync(string email)
    {
        email = email.ToLower().EncodeEmail();

        var (topLevelDomain, secondLevelDomain, domain, localPart, _) = email.SplitEmail();

        if (_secondLevelDomains.Contains(secondLevelDomain) && _topLevelDomains.Contains(topLevelDomain))
        {
            return Task.FromResult(new ValidationResult<TypoValidationResult>
            {
                IsValid = true,
                Message = "Provided email is valid",
                ValidationDetails = new TypoValidationResult
                {
                    Address = localPart,
                    Domain = domain,
                    OriginalEmail = $"{localPart}@{domain}"
                }
            });
        }

        var closestDomain = FindClosestDomain(domain, _domains, _domainThreshold);

        if (!string.IsNullOrWhiteSpace(closestDomain))
        {
            if (closestDomain == domain)
            {
                return Task.FromResult(new ValidationResult<TypoValidationResult>
                {
                    IsValid = true,
                    Message = "Provided email is valid",
                    ValidationDetails = new TypoValidationResult
                    {
                        Address = localPart,
                        Domain = domain,
                        OriginalEmail = $"{localPart}@{domain}"
                    }
                });
            }

            return Task.FromResult(new ValidationResult<TypoValidationResult>
            {
                IsValid = false,
                Message = "Provided email was invalid. Suggestion Provided",
                ValidationDetails = new TypoValidationResult
                {
                    Address = localPart,
                    Domain = closestDomain,
                    SuggestedEmail = $"{localPart}@{closestDomain}",
                    OriginalEmail = email
                }
            });
        }

        var closestSecondLevelDomain =
            FindClosestDomain(secondLevelDomain, _secondLevelDomains, _secondLevelThreshold);
        var closestTopLevelDomain = FindClosestDomain(topLevelDomain, _topLevelDomains, _topLevelThreshold);

        closestDomain = domain;
        var isTypo = false;

        if (!string.IsNullOrWhiteSpace(closestSecondLevelDomain) && closestSecondLevelDomain != secondLevelDomain)
        {
            closestDomain = closestDomain?.Replace(secondLevelDomain, closestSecondLevelDomain);
            isTypo = true;
        }

        if (!string.IsNullOrWhiteSpace(closestTopLevelDomain) && closestTopLevelDomain != topLevelDomain &&
            !string.IsNullOrEmpty(secondLevelDomain))
        {
            // TODO: originally a regex was used on top level domain. Needs testing, might not be needed.
            // TODO: closestDomain = closestDomain.replace(new RegExp(emailParts.topLevelDomain + "$"), closestTopLevelDomain);
            closestDomain = closestDomain?.Replace(topLevelDomain, closestTopLevelDomain);
            isTypo = true;
        }

        if (!isTypo)
        {
            return Task.FromResult(new ValidationResult<TypoValidationResult>
            {
                IsValid = true,
                Message = "Provided email is valid",
                ValidationDetails = new TypoValidationResult
                {
                    Address = localPart,
                    Domain = domain,
                    OriginalEmail = $"{localPart}@{domain}",
                    SuggestedEmail = $"{localPart}@{closestDomain}"
                }
            });
        }

        return Task.FromResult(new ValidationResult<TypoValidationResult>
        {
            IsValid = false,
            Message = "Provided email was invalid. Suggestion Provided",
            ValidationDetails = new TypoValidationResult
            {
                Address = localPart,
                Domain = closestDomain,
                SuggestedEmail = $"{localPart}@{closestDomain}",
                OriginalEmail = email
            }
        });
    }

    private static string FindClosestDomain(string domain, IReadOnlyList<string> domains, int threshold)
    {
        var minDist = double.PositiveInfinity;
        string closestDomain = null;

        if (string.IsNullOrWhiteSpace(domain) || domains.Count == 0)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        for (var i = 0; i < domains.Count; i++)
        {
            if (domain == domains[i])
            {
                return domain;
            }

            var dist = SiftForDistance(domain, domains[i]);

            if (!(dist < minDist)) continue;

            minDist = dist;
            closestDomain = domains[i];
        }

        if (minDist <= threshold && closestDomain != null)
        {
            return closestDomain;
        }

        return string.Empty;
    }

    // https://siderite.dev/blog/super-fast-and-accurate-string-distance-sift3.html/
    private static double CalculateSiftDistance(string string1, string string2, int maxOffset = 5)
    {
        if (string.IsNullOrWhiteSpace(string1))
        {
            return string.IsNullOrWhiteSpace(string2) ? 0 : string2.Length;
        }

        if (string.IsNullOrWhiteSpace(string2))
        {
            return string1.Length;
        }

        var length1 = string1.Length;
        var length2 = string2.Length;

        var cursor1 = 0; // cursor for string 1
        var cursor2 = 0; // cursor for string 2
        var largestCommonSubsequence = 0; // largest common subsequence
        var localCommonSubstring = 0; // local common substring
        var transpositions = 0; // number of transpositions ("ab" vs "ba")
        var offsetPairs = new List<CursorPosition>(); // offset pair array for computing the transposition

        while (cursor1 < length1 && cursor2 < length2)
        {
            if (string1[cursor1] == string2[cursor2])
            {
                localCommonSubstring++;
                var isTransposition = false;
                var i = 0;
                while (i < offsetPairs.Count)
                {
                    var offsetPair = offsetPairs[i];
                    if (cursor1 <= offsetPair.CursorPosition1 || cursor2 <= offsetPair.CursorPosition2)
                    {
                        isTransposition = Math.Abs(cursor2 - cursor1) >= Math.Abs(offsetPair.CursorPosition2 - offsetPair.CursorPosition1);

                        if (isTransposition)
                        {
                            transpositions++;
                        }
                        else if (!offsetPair.IsTrans)
                        {
                            offsetPair.IsTrans = true;
                            transpositions++;
                        }

                        break;
                    }

                    if (cursor1 > offsetPair.CursorPosition2 && cursor2 > offsetPair.CursorPosition1)
                    {
                        offsetPairs.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                offsetPairs.Add(new CursorPosition
                {
                    CursorPosition1 = cursor1,
                    CursorPosition2 = cursor2,
                    IsTrans = isTransposition
                });
            }
            else
            {
                largestCommonSubsequence += localCommonSubstring;
                localCommonSubstring = 0;
                if (cursor1 != cursor2)
                {
                    cursor1 = cursor2 = Math.Min(cursor1, cursor2);
                }

                for (var j = 0; j < maxOffset && (cursor1 + j < length1 || cursor2 + j < length2); j++)
                {
                    if (cursor1 + j < length1 && string1[cursor1 + j] == string2[cursor2])
                    {
                        cursor1 += j - 1;
                        cursor2--;
                        break;
                    }

                    if (cursor2 + j < length2 && string1[cursor1] == string2[cursor2 + j])
                    {
                        cursor1--;
                        cursor2 += j - 1;
                        break;
                    }
                }
            }

            cursor1++;
            cursor2++;

            if (cursor1 >= length1 || cursor2 >= length2)
            {
                largestCommonSubsequence += localCommonSubstring;
                localCommonSubstring = 0;
                cursor1 = cursor2 = Math.Min(cursor1, cursor2);
            }
        }

        largestCommonSubsequence += localCommonSubstring;
        return Math.Round((double)(Math.Max(length1, length2) - largestCommonSubsequence + transpositions));
    }
}

public sealed class CursorPosition
{
    public int CursorPosition1 { get; set; }
    public int CursorPosition2 { get; set; }
    public bool IsTrans { get; set; }
}