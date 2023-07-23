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
// File: EmailValidatorServiceCollectionExtensions.cs
// ---------------------------------------------------------------------------
#endregion

using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Validators;
using DnsClient;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedEmailValidator;

public static class EmailValidatorServiceCollectionExtensions
{
    public static IServiceCollection AddEmailValidator(this IServiceCollection services)
    {
        services.AddSingleton<ILookupClient, LookupClient>();

        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IBuildDependencies, BuildDependencies>();
        services.AddScoped<IDnsValidator, DnsValidator>();
        services.AddScoped<ITypoCheck, TypoCheck>();
        services.AddScoped<IRegexValidator, RegexValidator>();
        services.AddScoped<IDisposableValidator, DisposableValidator>();
        services.AddScoped<IEmailValidator, EmailValidator>();

        return services;
    }
}