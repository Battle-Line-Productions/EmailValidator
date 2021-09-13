namespace EmailValidator.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Net.NetworkInformation;
    using Models;

    public class TypoCheck
    {
        private readonly int _domainThreshold = 2;
        private readonly int _secondLevelThreshold = 2;
        private readonly int _topLevelThreshold = 2;

        private readonly List<string> _defaultDomains = new()
        {
            "msn.com", "bellsouth.net", "telus.net", "comcast.net", "optusnet.com.au", "earthlink.net", "qq.com",
            "sky.com", "icloud.com", "mac.com", "sympatico.ca", "googlemail.com", "att.net", "xtra.co.nz", "web.de",
            "cox.net", "gmail.com", "ymail.com", "aim.com", "rogers.com", "verizon.net", "rocketmail.com", "google.com",
            "optonline.net", "sbcglobal.net", "aol.com", "me.com", "btinternet.com", "charter.net", "shaw.ca"
        };
        private readonly List<string> _defaultSecondLevelDomains =
            new() { "yahoo", "hotmail", "mail", "live", "outlook", "gmx" };

        private readonly List<string> _defaultTopLevelDomains = new()
        {
            "com", "com.au", "com.tw", "ca", "co.nz", "co.uk", "de", "fr", "it", "ru", "net", "org", "edu", "gov", "jp",
            "nl", "kr", "se", "eu", "ie", "co.il", "us", "at", "be", "dk", "hk", "es", "gr", "ch", "no", "cz", "in",
            "net", "net.au", "info", "biz", "mil", "co.jp", "sg", "hu", "uk"
        };

        public TypoCheck(TypoOptions options)
        {
            _defaultDomains = options.Domains ?? _defaultDomains;
            _defaultSecondLevelDomains = options.SecondLevelDomains ?? _defaultSecondLevelDomains;
            _defaultTopLevelDomains = options.TopLevelDomains ?? _defaultTopLevelDomains;
        }

        private static int SiftForDistance(int stringOne = 0, int s2 = 0, int maxOffset = 5)
        {
            if (s1 == 0)
            {
                return s2 == 0 ? 0 : s2;
            }

            if (s2 == 0)
            {
                return s1;
            }

            var l1 = s1;
            var l2 = s2;

            var c1 = 0; // cursor for string 1
            var c2 = 0; // cursor for string 2
            var lcss = 0; // largest common subsequence
            var localCs = 0; // local common substring
            var trans = 0; // number of transpositions ("ab" vs "ba")
            var offsetArray = new List<int>(); // offset pair array for computing the transposition

            while (c2 >= l2 || c1 >= l1)
            {
                
            }

        }

    }
}