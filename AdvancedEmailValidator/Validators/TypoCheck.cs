namespace AdvancedEmailValidator.Validators
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using Models;

    public class TypoCheck
    {
        private readonly int _domainThreshold = 2;
        private readonly int _secondLevelThreshold = 2;
        private readonly int _topLevelThreshold = 2;

        private readonly List<string> _domains = new()
        {
            "msn.com", "bellsouth.net", "telus.net", "comcast.net", "optusnet.com.au", "earthlink.net", "qq.com",
            "sky.com", "icloud.com", "mac.com", "sympatico.ca", "googlemail.com", "att.net", "xtra.co.nz", "web.de",
            "cox.net", "gmail.com", "ymail.com", "aim.com", "rogers.com", "verizon.net", "rocketmail.com", "google.com",
            "optonline.net", "sbcglobal.net", "aol.com", "me.com", "btinternet.com", "charter.net", "shaw.ca"
        };
        private readonly List<string> _secondLevelDomains =
            new() { "yahoo", "hotmail", "mail", "live", "outlook", "gmx" };

        private readonly List<string> _topLevelDomains = new()
        {
            "com", "com.au", "com.tw", "ca", "co.nz", "co.uk", "de", "fr", "it", "ru", "net", "org", "edu", "gov", "jp",
            "nl", "kr", "se", "eu", "ie", "co.il", "us", "at", "be", "dk", "hk", "es", "gr", "ch", "no", "cz", "in",
            "net", "net.au", "info", "biz", "mil", "co.jp", "sg", "hu", "uk"
        };

        public TypoCheck(TypoOptions options)
        {
            _domains = options != null ? options.Domains ?? _domains : _domains;
            _secondLevelDomains = options != null ? options.SecondLevelDomains ?? _secondLevelDomains : _secondLevelDomains;
            _topLevelDomains = options != null ? options.TopLevelDomains ?? _topLevelDomains : _topLevelDomains;
            _domainThreshold = options != null ? options.DomainThreshold ?? _domainThreshold : _domainThreshold;
            _secondLevelThreshold = options != null ? options.SecondLevelThreshold ?? _secondLevelThreshold : _secondLevelThreshold;
            _topLevelThreshold = options != null ? options.TopLevelThreshold ?? _topLevelThreshold : _topLevelThreshold;
        }

        public ValidationResult<TypoValidationResult> Suggest(string email)
        {
            email = email.ToLower().EncodeEmail();

            var (topLevelDomain, secondLevelDomain, domain, localPart, _) = email.SplitEmail();

            if (_secondLevelDomains.Contains(secondLevelDomain) && _topLevelDomains.Contains(topLevelDomain))
            {
                return new ValidationResult<TypoValidationResult>
                {
                    IsValid = true,
                    Message = "Provided email is valid",
                    ValidationDetails = new TypoValidationResult
                    {
                        Address = localPart,
                        Domain = domain,
                        OriginalEmail = $"{localPart}@{domain}",
                    }
                };
            }

            var closestDomain = FindClosestDomain(domain, _domains, _domainThreshold);

            if (!string.IsNullOrWhiteSpace(closestDomain))
            {
                if (closestDomain == domain)
                {
                    return new ValidationResult<TypoValidationResult>
                    {
                        IsValid = true,
                        Message = "Provided email is valid",
                        ValidationDetails = new TypoValidationResult
                        {
                            Address = localPart,
                            Domain = domain,
                            OriginalEmail = $"{localPart}@{domain}",
                        }
                    };
                }

                return new ValidationResult<TypoValidationResult>
                {
                    IsValid = false,
                    Message = "Provided email was invalid. Suggestion Provided",
                    ValidationDetails = new TypoValidationResult
                    {
                        Address = localPart,
                        Domain = closestDomain,
                        SuggestedEmail = $"{localPart}@{closestDomain}",
                        OriginalEmail = email,
                    }
                };
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

            if (!string.IsNullOrWhiteSpace(closestTopLevelDomain) && closestTopLevelDomain != topLevelDomain && !string.IsNullOrEmpty(secondLevelDomain))
            {
                // TODO: originally a regex was used on top level domain. Needs testing, might not be needed.
                // TODO: closestDomain = closestDomain.replace(new RegExp(emailParts.topLevelDomain + "$"), closestTopLevelDomain);
                closestDomain = closestDomain?.Replace(topLevelDomain, closestTopLevelDomain);
                isTypo = true;
            }

            if (!isTypo)
            {
                return new ValidationResult<TypoValidationResult>
                {
                    IsValid = true,
                    Message = "Provided email is valid",
                    ValidationDetails = new TypoValidationResult
                    {
                        Address = localPart,
                        Domain = domain,
                        OriginalEmail = $"{localPart}@{domain}",
                        SuggestedEmail = $"{localPart}@{closestDomain}",
                    }
                };
            }

            return new ValidationResult<TypoValidationResult>
            {
                IsValid = false,
                Message = "Provided email was invalid. Suggestion Provided",
                ValidationDetails = new TypoValidationResult
                {
                    Address = localPart,
                    Domain = closestDomain,
                    SuggestedEmail = $"{localPart}@{closestDomain}",
                    OriginalEmail = email,
                }
            };
        }

        private static string FindClosestDomain(string domain, List<string> domains, int threshold)
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
        private static double SiftForDistance(string s1, string s2, int maxOffset = 5)
        {
            if (string.IsNullOrWhiteSpace(s1))
            {
                return string.IsNullOrWhiteSpace(s2) ? 0 : s2.Length;
            }

            if (string.IsNullOrWhiteSpace(s2))
            {
                return s1.Length;
            }

            var l1 = s1.Length;
            var l2 = s2.Length;

            var c1 = 0; // cursor for string 1
            var c2 = 0; // cursor for string 2
            var lcss = 0; // largest common subsequence
            var localCs = 0; // local common substring
            var trans = 0; // number of transpositions ("ab" vs "ba")
            var offsetArray = new List<CursorPosition>(); // offset pair array for computing the transposition

            while (c1 < l1 && c2 < l2)
            {
                if (s1[c1] == s2[c2])
                {
                    localCs++;
                    var isTrans = false;
                    var i = 0;
                    while (i < offsetArray.Count)
                    {
                        var ofs = offsetArray[i];
                        if (c1 <= ofs.CursorPosition1 || c2 <= ofs.CursorPosition2)
                        {
                            isTrans = Math.Abs(c2 - c1) >= Math.Abs(ofs.CursorPosition2 - ofs.CursorPosition1);

                            if (isTrans)
                            {
                                trans++;
                            }
                            else
                            {
                                if (!ofs.IsTrans)
                                {
                                    ofs.IsTrans = true;
                                    trans++;
                                }
                            }
                            break;
                        }

                        if (c1 > ofs.CursorPosition2 && c2 > ofs.CursorPosition1)
                        {
                            offsetArray.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    
                    offsetArray.Add(new CursorPosition
                    {
                        CursorPosition1 = c1,
                        CursorPosition2 = c2,
                        IsTrans = isTrans
                    });
                }
                else
                {
                    lcss += localCs;
                    localCs = 0;
                    if (c1 != c2)
                    {
                        c1 = c2 = Math.Min(c1, c2);
                    }

                    for (var j = 0; j < maxOffset && (c1 + j < l1 || c2 + j < l2); j++)
                    {
                        if (c1 + j < l1 && s1[c1 + j] == s2[c2])
                        {
                            c1 += j - 1;
                            c2--;
                            break;
                        }

                        if (c2 + j < l2 && s1[c1] == s2[c2 + j])
                        {
                            c1--;
                            c2 += j - 1;
                            break;
                        }
                    }
                }

                c1++;
                c2++;

                if (c1 >= l1 || c2 >= l2)
                {
                    lcss += localCs;
                    localCs = 0;
                    c1 = c2 = Math.Min(c1, c2);
                }
            }

            lcss += localCs;
            return Math.Round((double)(Math.Max(l1, l2) - lcss + trans));
        }
    }

    public sealed class CursorPosition
    {
        public int CursorPosition1 { get; set; }
        public int CursorPosition2 { get; set; }
        public bool IsTrans { get; set; }
    }
}