#region Copyright
// ---------------------------------------------------------------------------
// Copyright (c) 2023 BattleLine Productions LLC. All rights reserved.
// 
// Licensed under the BattleLine Productions LLC license agreement.
// See LICENSE file in the project root for full license information.
// 
// Author: Michael Cavanaugh
// Company: BattleLine Productions LLC
// Date: 07/23/2023
// Project: Frontline CRM
// File: EmailExtensionsTests.cs
// ---------------------------------------------------------------------------
#endregion

using System;
using AdvancedEmailValidator.Extensions;
using Xunit;

namespace UnitTests.AdvancedEmailValidator.Extensions;

public class EmailExtensionsTests
{
    [Theory]
    [InlineData("user@test.com", "test.com")]
    [InlineData("user@subdomain.test.com", "subdomain.test.com")]
    public void GetEmailDomain_ValidEmails_ReturnsExpectedDomain(string email, string expectedDomain)
    {
        var domain = email.GetEmailDomain();

        Assert.Equal(expectedDomain, domain);
    }

    [Theory]
    [InlineData("")]
    [InlineData("@")]
    [InlineData("user")]
    [InlineData("user@ ")]
    [InlineData(" user@")]
    [InlineData("user@.com")]
    public void GetEmailDomain_InvalidEmails_ThrowsArgumentException(string email)
    {
        Assert.Throws<ArgumentException>(() => email.GetEmailDomain());
    }

    [Theory]
    [InlineData("user@test.com", "com", "test", "test.com", "user", "user@test.com")]
    [InlineData("user@subdomain.test.com", "com", "subdomain", "subdomain.test.com", "user", "user@subdomain.test.com")]
    public void SplitEmail_ValidEmails_ReturnsExpectedResult(string email, string expectedTLD, string expectedSLD, string expectedDomain, string expectedEmailPart, string expectedFullAddress)
    {
        var (tld, sld, domain, emailPart, fullAddress) = email.SplitEmail();

        Assert.Equal(expectedTLD, tld);
        Assert.Equal(expectedSLD, sld);
        Assert.Equal(expectedDomain, domain);
        Assert.Equal(expectedEmailPart, emailPart);
        Assert.Equal(expectedFullAddress, fullAddress);
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentNullException))]
    [InlineData("user", typeof(ArgumentException))]
    [InlineData("user@", typeof(ArgumentException))]
    [InlineData("user@.com", typeof(ArgumentException))]
    [InlineData("user@domain", typeof(ArgumentException))]
    public void SplitEmail_InvalidEmails_ThrowsException(string email, Type expectedExceptionType)
    {
        Assert.Throws(expectedExceptionType, () => email.SplitEmail());
    }

    [Theory]
    [InlineData("user@test.com", "user%40test.com")]
    [InlineData("user with spaces@test.com", "userwithspaces%40test.com")]
    public void EncodeEmail_ValidEmails_ReturnsExpectedResult(string email, string expectedResult)
    {
        var result = email.EncodeEmail();

        Assert.Equal(expectedResult, result);
    }
}