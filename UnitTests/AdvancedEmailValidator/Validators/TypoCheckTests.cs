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
// File: TypoCheckTests.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedEmailValidator.Models;
using AdvancedEmailValidator.Validators;
using Xunit;

#endregion

namespace UnitTests.AdvancedEmailValidator.Validators;

public class TypoCheckTests
{
    private readonly TypoCheck _typoCheck;

    public TypoCheckTests()
    {
        var domains = new List<string>
            { "google.com", "gmail.com", "emaildomain.com", "comcast.net", "facebook.com", "msn.com" };
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
    public async Task Suggest_IsCalledWithValidEmail_ReturnsValidationResponseSuccessfully()
    {
        const string validEmail = "email@msn.com";

        var result = await _typoCheck.SuggestAsync(validEmail);

        Assert.Null(result.ValidationDetails.SuggestedEmail);
        Assert.True(result.IsValid);
        Assert.Equal(validEmail, result.ValidationDetails.OriginalEmail);
        Assert.Equal("email", result.ValidationDetails.Address);
        Assert.Equal("msn.com", result.ValidationDetails.Domain);
        Assert.Equal("Provided email is valid", result.Message);
    }

    [Fact]
    public async Task Suggest_IsCalledWithInValidEmail_ReturnsValidationResponseSuccessfully()
    {
        var invalidEmail = "email@msm.com";

        var result = await _typoCheck.SuggestAsync(invalidEmail);

        Assert.Equal("email@msn.com", result.ValidationDetails.SuggestedEmail);
        Assert.False(result.IsValid);
        Assert.Equal(invalidEmail, result.ValidationDetails.OriginalEmail);
        Assert.Equal("email", result.ValidationDetails.Address);
        Assert.Equal("msn.com", result.ValidationDetails.Domain);
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
    [InlineData("test@yajoo.com", "test@ymail.com", "test", "ymail.com")]
    [InlineData("test@randomsmallcompany.cmo", "test@randomsmallcompany.com", "test", "randomsmallcompany.com")]
    public async Task Suggest_IsCalledWithInValidEmails_ReturnsValidationResponseSuccessfully(
        string invalidEmail,
        string correctedEmail,
        string localPart,
        string expectedDomainPart)
    {
        var result = await _typoCheck.SuggestAsync(invalidEmail);

        Assert.Equal(correctedEmail, result.ValidationDetails.SuggestedEmail);
        Assert.False(result.IsValid);
        Assert.Equal(invalidEmail.ToLower(), result.ValidationDetails.OriginalEmail);
        Assert.Equal(localPart, result.ValidationDetails.Address);
        Assert.Equal(expectedDomainPart, result.ValidationDetails.Domain);
        Assert.Equal("Provided email was invalid. Suggestion Provided", result.Message);
    }
}