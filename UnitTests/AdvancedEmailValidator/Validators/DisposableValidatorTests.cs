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
// File: DisposableValidatorTests.cs
// ---------------------------------------------------------------------------
#endregion

using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Validators;
using System;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;

namespace UnitTests.AdvancedEmailValidator.Validators;

public class DisposableValidatorTests
{
    private readonly IFileReader _fileReader;
    private readonly DisposableValidator _validator;

    public DisposableValidatorTests()
    {
        _fileReader = A.Fake<IFileReader>();
        A.CallTo(() => _fileReader.Exists(A<string>.Ignored)).Returns(true);

        _validator = new DisposableValidator(_fileReader);
    }

    [Fact]
    public async Task ValidateAsync_EmailNotInDisposableList_ReturnsIsValidTrue()
    {
        A.CallTo(() => _fileReader.ReadAllLinesAsync(A<string>.Ignored)).Returns(Array.Empty<string>());

        var result = await _validator.ValidateAsync($"test.com");

        Assert.True(result.IsValid);
        A.CallTo(() => _fileReader.ReadAllLinesAsync(A<string>.Ignored)).MustHaveHappened();
    }

    [Fact]
    public async Task ValidateAsync_EmailInDisposableList_ReturnsIsValidFalse()
    {
        A.CallTo(() => _fileReader.ReadAllLinesAsync(A<string>.Ignored)).Returns(new[] { "test.com" });

        var result = await _validator.ValidateAsync("test@test.com");

        Assert.False(result.IsValid);
        A.CallTo(() => _fileReader.ReadAllLinesAsync(A<string>.Ignored)).MustHaveHappened();
    }
}
