namespace UnitTests.EmailValidator.Validators
{
    using System.Collections.Generic;
    using global::EmailValidator.Models;
    using global::EmailValidator.Validators;
    using Xunit;

    public class TypoCheckTests
    {
        private readonly TypoCheck _typoCheck;

        public TypoCheckTests()
        {
            var domains = new List<string> { "google.com", "gmail.com", "emaildomain.com", "comcast.net", "facebook.com", "msn.com" };
            var secondLevelDomains = new List<string> { "yahoo", "hotmail", "mail", "live", "outlook", "gmx" };
            var topLevelDomains = new List<string> { "co.uk", "com", "org", "info", "fr" };

            var typoOptions = new TypoOptions
            {
                Domains = domains,
                SecondLevelDomains = secondLevelDomains,
                TopLevelDomains = topLevelDomains
            };

            _typoCheck = new TypoCheck(typoOptions);
        }

        [Fact]
        public void Suggest_IsCalledWithValidEmail_ReturnsValidationResponseSuccessfully()
        {
            var validEmail = "email@msn.com";

            var result = _typoCheck.Suggest(validEmail);
            
            Assert.Null(result.SuggestedEmail);
            Assert.True(result.IsValid);
            Assert.Equal(validEmail, result.OriginalEmail);
            Assert.Equal("email", result.Address);
            Assert.Equal("msn.com", result.Domain);
            Assert.Equal("Provided email is valid", result.Message);
        }
        
        [Fact]
        public void Suggest_IsCalledWithInValidEmail_ReturnsValidationResponseSuccessfully()
        {
            var invalidEmail = "email@msm.com";

            var result = _typoCheck.Suggest(invalidEmail);
            
            Assert.Equal("email@msn.com", result.SuggestedEmail);
            Assert.False(result.IsValid);
            Assert.Equal(invalidEmail, result.OriginalEmail);
            Assert.Equal("email", result.Address);
            Assert.Equal("msn.com", result.Domain);
            Assert.Equal("Provided email was invalid. Suggestion Provided", result.Message);
        }
        
        [Theory]
        [InlineData("test@gmailc.om", "test@gmail.com", "test", "gmail.com")]
        [InlineData("test@emaildomain.co", "test@emaildomain.com", "test", "emaildomain.com")]
        [InlineData("test@gmail.con", "test@gmail.com", "test", "gmail.com")]
        [InlineData("test@gnail.con", "test@gmail.com", "test", "gmail.com")]
        [InlineData("test@GNAIL.con", "test@gmail.com", "test", "gmail.com")]
        [InlineData("test@#gmail.com", "test@gmail.com", "test", "gmail.com")]
        [InlineData("test@comcast.nry", "test@comcast.net", "test", "comcast.net")]
        [InlineData("test@homail.con", "test@hotmail.com", "test", "hotmail.com")]
        [InlineData("test@hotmail.co", "test@hotmail.com", "test", "hotmail.com")]
        [InlineData("test@yajoo.com", "test@yahoo.com", "test", "yahoo.com")]
        [InlineData("test@randomsmallcompany.cmo", "test@randomsmallcompany.com", "test", "randomsmallcompany.com")]
        public void Suggest_IsCalledWithInValidEmails_ReturnsValidationResponseSuccessfully(
            string invalidEmail, 
            string correctedEmail, 
            string localPart, 
            string expectedDomainPart)
        {
            var result = _typoCheck.Suggest(invalidEmail);
            
            Assert.Equal(correctedEmail, result.SuggestedEmail);
            Assert.False(result.IsValid);
            Assert.Equal(invalidEmail.ToLower(), result.OriginalEmail);
            Assert.Equal(localPart, result.Address);
            Assert.Equal(expectedDomainPart, result.Domain);
            Assert.Equal("Provided email was invalid. Suggestion Provided", result.Message);
        }
    }
}