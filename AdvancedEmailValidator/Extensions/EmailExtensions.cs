namespace AdvancedEmailValidator.Extensions
{
    using System;
    using System.Linq;

    public static class EmailExtensions
    {
        public static string GetEmailDomain(this string email)
        {
            return email.Split("@")[1];
        }

        public static (string, string, string, string, string) SplitEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
            email = email.Trim();

            var emailParts = email.Split("@");

            if (emailParts.Length < 2 || email.Any(x => string.IsNullOrWhiteSpace(x.ToString())))
            {
                throw new ArgumentException($"{email} is not a valid email");
            }

            var domain = emailParts.Last();
            var domainParts = domain.Split(".");
            var secondLevelDomain = string.Empty;
            var topLevelDomain = string.Empty;

            switch (domainParts.Length)
            {
                case 0:
                    throw new ArgumentException("Email is missing a top level domain");
                case 1:
                    topLevelDomain = domainParts[0];
                    break;
                default:
                {
                    secondLevelDomain = domainParts[0];
                    for (var i = 1; i < domainParts.Length; i++)
                    {
                        topLevelDomain += domainParts[i] + ".";
                    }

                    topLevelDomain = topLevelDomain.Substring(0, topLevelDomain.Length - 1);
                    break;
                }
            }

            var fullAddress = string.Join("@", emailParts);

            return (topLevelDomain, secondLevelDomain, domain, emailParts[0], fullAddress);
        }

        public static string EncodeEmail(this string email)
        {
            // http://en.wikipedia.org/wiki/Email_address#Syntax
            var result = Uri.EscapeUriString(email);
            return result.Replace("%20", "")
                .Replace("%25", "%")
                .Replace("%5E", "^")
                .Replace("%60", "`")
                .Replace("%7B", "{")
                .Replace("%7C", "|")
                .Replace("%7D", "}");
        }
    }
}