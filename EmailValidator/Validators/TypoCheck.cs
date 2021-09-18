namespace EmailValidator.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
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
            _domains = options.Domains ?? _domains;
            _secondLevelDomains = options.SecondLevelDomains ?? _secondLevelDomains;
            _topLevelDomains = options.TopLevelDomains ?? _topLevelDomains;
            _domainThreshold = options.DomainThreshold ?? _domainThreshold;
            _secondLevelThreshold = options.SecondLevelThreshold ?? _secondLevelThreshold;
            _topLevelThreshold = options.TopLevelThreshold ?? _topLevelThreshold;
        }

        public ValidationResult Suggest(string email)
        {
            email = email.ToLower().EncodeEmail();

            var (topLevelDomain, secondLevelDomain, domain, address) = email.SplitEmail();

            if (_secondLevelDomains.Contains(secondLevelDomain) && _topLevelDomains.Contains(topLevelDomain))
            {
                return new TypoValidationResult
                {
                    IsValid = true,
                    Address = address,
                    Domain = domain,
                    OriginalEmail = $"{address}@{domain}]",
                    Message = "Provided email is valid"
                };
            }

            var closestDomain = FindClosestDomain(domain, _domains, _domainThreshold);

            if (!string.IsNullOrWhiteSpace(closestDomain))
            {
                if (closestDomain == domain)
                {
                    return new TypoValidationResult
                    {
                        IsValid = true,
                        Address = address,
                        Domain = domain,
                        OriginalEmail = $"{address}@{domain}]",
                        Message = "Provided email is valid"
                    };
                }

                return new TypoValidationResult
                {
                    IsValid = false,
                    Address = address,
                    Domain = closestDomain,
                    SuggestedEmail = $"{address}@{closestDomain}]",
                    OriginalEmail = email,
                    Message = "Provided email was invalid. Suggestion Provided"
                };
            }

            var closestSecondLevelDomain =
                FindClosestDomain(secondLevelDomain, _secondLevelDomains, _secondLevelThreshold);
            var closestTopLevelDomain = FindClosestDomain(topLevelDomain, _topLevelDomains, _topLevelThreshold);

            var isTypo = false;

            if (string.IsNullOrWhiteSpace(closestSecondLevelDomain) && closestSecondLevelDomain != secondLevelDomain)
            {
                closestDomain = closestDomain?.Replace(secondLevelDomain, closestSecondLevelDomain);
                isTypo = true;
            }

            if (string.IsNullOrWhiteSpace(closestTopLevelDomain) && closestTopLevelDomain != topLevelDomain)
            {
                // TODO: originally a regex was used on top level domain. Needs testing, might not be needed.
                // TODO: closestDomain = closestDomain.replace(new RegExp(emailParts.topLevelDomain + "$"), closestTopLevelDomain);
                closestDomain = closestDomain?.Replace(topLevelDomain, closestTopLevelDomain);
                isTypo = true;
            }

            if (!isTypo)
            {
                return new TypoValidationResult
                {
                    IsValid = true,
                    Address = address,
                    Domain = domain,
                    OriginalEmail = $"{address}@{domain}]",
                    Message = "Provided email is valid"
                };
            }
            
            return new TypoValidationResult
            {
                IsValid = false,
                Address = address,
                Domain = closestDomain,
                SuggestedEmail = $"{address}@{closestDomain}]",
                OriginalEmail = email,
                Message = "Provided email was invalid. Suggestion Provided"
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

            foreach (var x in domains)
            {
                if (domain == x)
                {
                    return domain;
                }

                var dist = SiftForDistance(domain, x);
                
                if (!(dist < minDist)) continue;
                
                minDist = dist;
                closestDomain = x;
            }

            if (minDist <= threshold && closestDomain != null)
            {
                return closestDomain;
            }

            return string.Empty;
        }

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

            var l1 = s1;
            var l2 = s2;

            var c1 = 0; // cursor for string 1
            var c2 = 0; // cursor for string 2
            var lcss = 0; // largest common subsequence
            var localCs = 0; // local common substring
            var trans = 0; // number of transpositions ("ab" vs "ba")
            var offsetArray = new List<CursorPosition>(); // offset pair array for computing the transposition

            while (c1 < l1.Length && c2 < l2.Length)
            {
                if (s1[c1] == s2[c2])
                {
                    localCs++;
                    var isTrans = false;
                    var i = 0;
                    while (i <= offsetArray.Count)
                    {
                        var ofs = offsetArray[i];
                        if (c1 <= ofs.CursorPosition1 || c2 <= ofs.CursorPosition2)
                        {
                            isTrans = Math.Abs(c2 - c1) >= Math.Abs(ofs.CursorPosition1 - ofs.CursorPosition2);

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

                    for (int j = 0; j < maxOffset && (c1 + j < l1.Length || c2 + j < l2.Length); j++)
                    {
                        if (c1 + j < l1.Length && (s1[c1 + j] == s2[c2]))
                        {
                            c1 += j - 1;
                            c2--;
                            break;
                        }

                        if (c2 + j < l2.Length && (s1[c1] == s2[c2 + j]))
                        {
                            c1--;
                            c2 += j - 1;
                        }
                    }
                }

                c1++;
                c2++;

                if ((c1 >= l1.Length) || (c2 >= l2.Length))
                {
                    lcss += localCs;
                    localCs = 0;
                    c1 = c2 = Math.Min(c1, c2);
                }
            }

            lcss += localCs;
            return Math.Round((double)(Math.Max(l1.Length, l2.Length) - lcss + trans));
        }

    }

    public sealed class CursorPosition
    {
        public int CursorPosition1 { get; set; }
        public int CursorPosition2 { get; set; }
        public bool IsTrans { get; set; }
    }
}