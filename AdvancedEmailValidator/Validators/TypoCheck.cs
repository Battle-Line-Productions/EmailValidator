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
    private List<string> _domains = new()
    {
        "msn.com", "bellsouth.net", "telus.net", "comcast.net", "optusnet.com.au", "earthlink.net", "qq.com",
        "sky.com", "icloud.com", "mac.com", "sympatico.ca", "googlemail.com", "att.net", "xtra.co.nz", "web.de",
        "cox.net", "gmail.com", "ymail.com", "aim.com", "rogers.com", "verizon.net", "rocketmail.com", "google.com",
        "optonline.net", "sbcglobal.net", "aol.com", "me.com", "btinternet.com", "charter.net", "shaw.ca"
    };

    private int _domainThreshold = 2;

    private List<string> _secondLevelDomains =
        new() { "yahoo", "hotmail", "mail", "live", "outlook", "gmx" };

    private int _secondLevelThreshold = 2;

    private List<string> _topLevelDomains = new()
    {
        "com", "com.au", "com.tw", "ca", "co.nz", "co.uk", "de", "fr", "it", "ru", "net", "org", "edu", "gov", "jp",
        "nl", "kr", "se", "eu", "ie", "co.il", "us", "at", "be", "dk", "hk", "es", "gr", "ch", "no", "cz", "in",
        "net", "net.au", "info", "biz", "mil", "co.jp", "sg", "hu", "uk"
    };

    private int _topLevelThreshold = 2;

    private const int DefaultMaxOffset = 5;

    public TypoCheck()
    { }

    private void InitOptions(TypoOptions options)
    {
        _domains ??= options?.Domains ?? _domains;
        _secondLevelDomains ??= options?.SecondLevelDomains ?? _secondLevelDomains;
        _topLevelDomains ??= options?.TopLevelDomains ?? _topLevelDomains;
        _domainThreshold = options?.DomainThreshold ?? _domainThreshold;
        _secondLevelThreshold = options?.SecondLevelThreshold ?? _secondLevelThreshold;
        _topLevelThreshold = options?.TopLevelThreshold ?? _topLevelThreshold;
    }

    public Task<ValidationResult<TypoValidationResult>> SuggestAsync(string email, TypoOptions options = null)
    {
        InitOptions(options);

        email = email.ToLower();

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

            var dist = CalculateSiftDistance(domain, domains[i]);

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
    private static double CalculateSiftDistance(string sourceString, string targetString, int maxOffset = DefaultMaxOffset)
    {
        if (string.IsNullOrWhiteSpace(sourceString))
        {
            return string.IsNullOrWhiteSpace(targetString) ? 0 : targetString.Length;
        }

        if (string.IsNullOrWhiteSpace(targetString))
        {
            return sourceString.Length;
        }

        var sourceLength = sourceString.Length;
        var targetLength = targetString.Length;

        var sourceCursor = 0;
        var targetCursor = 0;
        var largestCommonSubsequence = 0;
        var localCommonSubstring = 0;
        var transpositions = 0;
        var offsetPairs = new List<CursorPosition>();

        while (sourceCursor < sourceLength && targetCursor < targetLength)
        {
            if (sourceString[sourceCursor] == targetString[targetCursor])
            {
                localCommonSubstring++;
                ProcessMatchingCharacters();
            }
            else
            {
                largestCommonSubsequence += localCommonSubstring;
                localCommonSubstring = 0;
                AdjustCursorsForNonMatchingCharacters();
            }

            sourceCursor++;
            targetCursor++;

            if (sourceCursor >= sourceLength || targetCursor >= targetLength)
            {
                largestCommonSubsequence += localCommonSubstring;
                localCommonSubstring = 0;
                sourceCursor = targetCursor = Math.Min(sourceCursor, targetCursor);
            }
        }

        largestCommonSubsequence += localCommonSubstring;
        return Math.Round((double)(Math.Max(sourceLength, targetLength) - largestCommonSubsequence + transpositions));

        void ProcessMatchingCharacters()
        {
            var isTransposition = false;
            var i = 0;
            while (i < offsetPairs.Count)
            {
                var offsetPair = offsetPairs[i];
                if (sourceCursor <= offsetPair.SourcePosition || targetCursor <= offsetPair.TargetPosition)
                {
                    isTransposition = Math.Abs(targetCursor - sourceCursor) >= Math.Abs(offsetPair.TargetPosition - offsetPair.SourcePosition);

                    if (isTransposition)
                    {
                        transpositions++;
                    }
                    else
                    {
                        if (!offsetPair.IsTransposition)
                        {
                            offsetPair.IsTransposition = true;
                            transpositions++;
                        }
                    }

                    break;
                }

                if (sourceCursor > offsetPair.TargetPosition && targetCursor > offsetPair.SourcePosition)
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
                SourcePosition = sourceCursor,
                TargetPosition = targetCursor,
                IsTransposition = isTransposition
            });
        }

        void AdjustCursorsForNonMatchingCharacters()
        {
            if (sourceCursor != targetCursor)
            {
                sourceCursor = targetCursor = Math.Min(sourceCursor, targetCursor);
            }

            for (var j = 0; j < maxOffset && (sourceCursor + j < sourceLength || targetCursor + j < targetLength); j++)
            {
                if (sourceCursor + j < sourceLength && sourceString[sourceCursor + j] == targetString[targetCursor])
                {
                    sourceCursor += j - 1;
                    targetCursor--;
                    break;
                }

                if (targetCursor + j < targetLength && sourceString[sourceCursor] == targetString[targetCursor + j])
                {
                    sourceCursor--;
                    targetCursor += j - 1;
                    break;
                }
            }
        }
    }
}

public sealed class CursorPosition
{
    public int SourcePosition { get; set; }
    public int TargetPosition { get; set; }
    public bool IsTransposition { get; set; }
}