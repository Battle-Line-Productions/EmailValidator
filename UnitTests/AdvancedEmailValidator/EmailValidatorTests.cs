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
// File: EmailValidatorTests.cs
// ---------------------------------------------------------------------------
#endregion

using System.Text.RegularExpressions;
using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Models;
using AdvancedEmailValidator;
using FakeItEasy;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.AdvancedEmailValidator;

public class EmailValidatorTests
{
    private readonly IDnsValidator _fakeDnsValidator = A.Fake<IDnsValidator>();
    private readonly ITypoCheck _fakeTypoCheck = A.Fake<ITypoCheck>();
    private readonly IRegexValidator _fakeRegexValidator = A.Fake<IRegexValidator>();
    private readonly IDisposableValidator _fakeDisposableValidator = A.Fake<IDisposableValidator>();
    private readonly IBuildDependencies _fakeBuildDependencies = A.Fake<IBuildDependencies>();

    private const string TestEmail = "test@test.com";

    public EmailValidatorTests()
    {
        A.CallTo(() => _fakeDnsValidator.QueryAsync(A<string>.Ignored)).Returns(Task.FromResult(new ValidationResult<DnsValidationResult>()));
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(A<string>.Ignored)).Returns(Task.FromResult(new ValidationResult<TypoValidationResult> { ValidationDetails = new TypoValidationResult() }));
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(A<string>.Ignored)).Returns(Task.FromResult(new ValidationResult<RegexValidationResult>()));
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(A<string>.Ignored, A<Regex>.Ignored)).Returns(Task.FromResult(new ValidationResult<RegexValidationResult>()));
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(A<string>.Ignored)).Returns(Task.FromResult(new ValidationResult<DisposableValidationResult>()));
        A.CallTo(() => _fakeBuildDependencies.CheckDependencies()).Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task ValidateAsync_WithAllOptionsEnabled_CallsAllValidators()
    {
        var options = new ValidationOptions
        {
            IsStrict = true,
            ValidateDisposable = true,
            ValidateMx = true,
            ValidateRegex = true,
            ValidateSimpleRegex = true,
            ValidateTypo = true
        };

        var emailValidator = new EmailValidator(_fakeDnsValidator, _fakeTypoCheck, _fakeRegexValidator, _fakeDisposableValidator, _fakeBuildDependencies);

        var result = await emailValidator.ValidateAsync(TestEmail, options);

        A.CallTo(() => _fakeDnsValidator.QueryAsync(TestEmail)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(TestEmail)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(TestEmail)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(TestEmail, A<Regex>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(TestEmail)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ValidateAsync_WithNoOptionsEnabled_CallsNoValidators()
    {
        var options = new ValidationOptions
        {
            IsStrict = true,
            ValidateDisposable = false,
            ValidateMx = false,
            ValidateRegex = false,
            ValidateSimpleRegex = false,
            ValidateTypo = false
        };

        var emailValidator = new EmailValidator(_fakeDnsValidator, _fakeTypoCheck, _fakeRegexValidator, _fakeDisposableValidator, _fakeBuildDependencies);

        var result = await emailValidator.ValidateAsync(TestEmail, options);

        A.CallTo(() => _fakeDnsValidator.QueryAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(TestEmail, A<Regex>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(TestEmail)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyMxEnabled_CallsOnlyDnsValidator()
    {
        var options = new ValidationOptions
        {
            IsStrict = true,
            ValidateDisposable = false,
            ValidateMx = true,
            ValidateRegex = false,
            ValidateSimpleRegex = false,
            ValidateTypo = false
        };

        var emailValidator = new EmailValidator(_fakeDnsValidator, _fakeTypoCheck, _fakeRegexValidator, _fakeDisposableValidator, _fakeBuildDependencies);

        var result = await emailValidator.ValidateAsync(TestEmail, options);

        A.CallTo(() => _fakeDnsValidator.QueryAsync(TestEmail)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(TestEmail, A<Regex>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(TestEmail)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyDisposableEnabled_CallsOnlyDisposableValidator()
    {
        var options = new ValidationOptions
        {
            IsStrict = true,
            ValidateDisposable = true,
            ValidateMx = false,
            ValidateRegex = false,
            ValidateSimpleRegex = false,
            ValidateTypo = false
        };

        var emailValidator = new EmailValidator(_fakeDnsValidator, _fakeTypoCheck, _fakeRegexValidator, _fakeDisposableValidator, _fakeBuildDependencies);

        var result = await emailValidator.ValidateAsync(TestEmail, options);

        A.CallTo(() => _fakeDnsValidator.QueryAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(TestEmail, A<Regex>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(TestEmail)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyRegexEnabled_CallsOnlyRegexValidator()
    {
        var options = new ValidationOptions
        {
            IsStrict = true,
            ValidateDisposable = false,
            ValidateMx = false,
            ValidateRegex = true,
            ValidateSimpleRegex = false,
            ValidateTypo = false
        };

        var emailValidator = new EmailValidator(_fakeDnsValidator, _fakeTypoCheck, _fakeRegexValidator, _fakeDisposableValidator, _fakeBuildDependencies);

        var result = await emailValidator.ValidateAsync(TestEmail, options);

        A.CallTo(() => _fakeDnsValidator.QueryAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(TestEmail, A<Regex>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(TestEmail)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ValidateAsync_WithOnlySimpleRegexEnabled_CallsOnlySimpleRegexValidator()
    {
        var options = new ValidationOptions
        {
            IsStrict = true,
            ValidateDisposable = false,
            ValidateMx = false,
            ValidateRegex = false,
            ValidateSimpleRegex = true,
            ValidateTypo = false
        };

        var emailValidator = new EmailValidator(_fakeDnsValidator, _fakeTypoCheck, _fakeRegexValidator, _fakeDisposableValidator, _fakeBuildDependencies);

        var result = await emailValidator.ValidateAsync(TestEmail, options);

        A.CallTo(() => _fakeDnsValidator.QueryAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(TestEmail)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(TestEmail, A<Regex>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(TestEmail)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyTypoCheckEnabled_CallsOnlyTypoCheckValidator()
    {
        var options = new ValidationOptions
        {
            IsStrict = true,
            ValidateDisposable = false,
            ValidateMx = false,
            ValidateRegex = false,
            ValidateSimpleRegex = false,
            ValidateTypo = true
        };

        var emailValidator = new EmailValidator(_fakeDnsValidator, _fakeTypoCheck, _fakeRegexValidator, _fakeDisposableValidator, _fakeBuildDependencies);

        var result = await emailValidator.ValidateAsync(TestEmail, options);

        A.CallTo(() => _fakeDnsValidator.QueryAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeTypoCheck.SuggestAsync(TestEmail)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRegexValidator.IsValidSimpleAsync(TestEmail)).MustNotHaveHappened();
        A.CallTo(() => _fakeRegexValidator.IsValidAsync(TestEmail, A<Regex>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeDisposableValidator.ValidateAsync(TestEmail)).MustNotHaveHappened();
    }
}