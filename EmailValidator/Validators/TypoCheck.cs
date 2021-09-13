namespace EmailValidator.Validators
{
    using System.Collections.Generic;
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

    }
}