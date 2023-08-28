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
using System;

namespace AdvancedEmailValidator;

public static class EmailValidatorServiceCollectionExtensions
{
    private const string DisposableEmailClientName = "DisposableEmailClient";

    public static IServiceCollection AddEmailValidator(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // ILookupClient should always be Singleton
        services.AddSingleton<ILookupClient, LookupClient>();
        services.AddHttpClient(DisposableEmailClientName);

        // Add other services
        AddService<IFileReader, FileReader>(services, lifetime);
        AddService<IBuildDependencies, BuildDependencies>(services, lifetime);
        AddService<IDnsValidator, DnsValidator>(services, lifetime);
        AddService<ITypoCheck, TypoCheck>(services, lifetime);
        AddService<IRegexValidator, RegexValidator>(services, lifetime);
        AddService<IDisposableValidator, DisposableValidator>(services, lifetime);
        AddService<IEmailValidator, EmailValidator>(services, lifetime);

        return services;
    }

    private static void AddService<TService, TImplementation>(IServiceCollection services, ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<TService, TImplementation>();
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped<TService, TImplementation>();
                break;
            case ServiceLifetime.Transient:
                services.AddTransient<TService, TImplementation>();
                break;
            default:
                throw new ArgumentException("Invalid service lifetime specified.");
        }
    }
}
