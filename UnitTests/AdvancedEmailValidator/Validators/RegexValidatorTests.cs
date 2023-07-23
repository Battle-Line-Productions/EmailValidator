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
// File: RegexValidatorTests.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Validators;
using System;
using System.Threading.Tasks;
using Xunit;

#endregion

namespace UnitTests.AdvancedEmailValidator.Validators;

public class RegexValidatorTests
{
    private readonly IRegexValidator _regexValidator;

    public RegexValidatorTests()
    {
        _regexValidator = new RegexValidator();
    }

    [Theory]
    [InlineData("@majjf.com", false)]
    [InlineData("A@b@c@example.com", false)]
    [InlineData("Abc.example.com", false)]
    [InlineData("j..s@proseware.com", true)]
    [InlineData("j.@server1.proseware.com", true)]
    [InlineData("js*@proseware.com", false)]
    [InlineData("js@proseware..com", false)]
    [InlineData("ma...ma@jjf.co", true)]
    [InlineData("ma.@jjf.com", true)]
    [InlineData("ma@@jjf.com", false)]
    [InlineData("ma@jjf.", false)]
    [InlineData("ma@jjf..com", false)]
    [InlineData("ma@jjf.c", false)]
    [InlineData("ma_@jjf", false)]
    [InlineData("ma_@jjf.", false)]
    [InlineData("ma_@jjf.com", true)]
    [InlineData("-------", false)]
    [InlineData("12@hostname.com", true)]
    [InlineData("d.j@server1.proseware.com", true)]
    [InlineData("david.jones@proseware.com", true)]
    [InlineData("j.s@server1.proseware.com", true)]
    [InlineData("j@proseware.com9", false)]
    [InlineData("j_9@[129.126.118.1]", true)]
    [InlineData("jones@ms1.proseware.com", true)]
    [InlineData("js#internal@proseware.com", false)]
    [InlineData("js@proseware.com9", false)]
    [InlineData("m.a@hostname.co", true)]
    [InlineData("m_a1a@hostname.com", true)]
    [InlineData("ma.h.saraf.onemore@hostname.com.edu", true)]
    [InlineData("ma@hostname.com", true)]
    [InlineData("ma@hostname.comcom", false)]
    [InlineData("MA@hostname.coMCom", false)]
    [InlineData("ma12@hostname.com", true)]
    [InlineData("ma-a.aa@hostname.com.edu", true)]
    [InlineData("ma-a@hostname.com", true)]
    [InlineData("ma-a@hostname.com.edu", true)]
    [InlineData("ma-a@1hostname.com", true)]
    [InlineData("ma.a@1hostname.com", true)]
    [InlineData("ma@1hostname.com", true)]
    public async Task IsValid_IsCalledWithAnEmail_ReturnsValidationResponseSuccessfully(string email, bool isValid)
    {
        var result = await _regexValidator.IsValidAsync(email, null);

        Assert.Equal(isValid, result.IsValid);
    }

    [Fact]
    public async Task IsValid_EmailIsEmpty_ReturnsFalse()
    {
        var result = await _regexValidator.IsValidSimpleAsync("");
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task IsValid_EmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _regexValidator.IsValidSimpleAsync(null));
    }

    [Fact]
    public async Task IsValid_EmailIsWhitespace_ReturnsFalse()
    {
        var result = await _regexValidator.IsValidSimpleAsync(" ");
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task IsValid_EmailHasNoAtSymbol_ReturnsFalse()
    {
        var result = await _regexValidator.IsValidSimpleAsync("email.com");
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task IsValid_EmailHasNoDomain_ReturnsFalse()
    {
        var result = await _regexValidator.IsValidSimpleAsync("email@");
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task IsValid_EmailHasNoUsername_ReturnsFalse()
    {
        var result = await _regexValidator.IsValidSimpleAsync("@domain.com");
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task IsValid_EmailIsValid_ReturnsTrue()
    {
        var result = await _regexValidator.IsValidSimpleAsync("email@domain.com");
        Assert.True(result.IsValid);
    }
}