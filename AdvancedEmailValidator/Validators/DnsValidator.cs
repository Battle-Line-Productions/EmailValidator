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
// File: DnsValidator.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using AdvancedEmailValidator.Extensions;
using AdvancedEmailValidator.Interfaces;
using AdvancedEmailValidator.Models;
using DnsClient;
using DnsClient.Protocol;

#endregion

namespace AdvancedEmailValidator.Validators;

public class DnsValidator : IDnsValidator
{
    private readonly ILookupClient _client;
    private readonly ValidationOptions _options;

    public DnsValidator(ValidationOptions options)
    {
        _options = options ?? new ValidationOptions();
        _client = new LookupClient();
    }

    public DnsValidator(ILookupClient client, ValidationOptions options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<ValidationResult<DnsValidationResult>> QueryAsync(string email)
    {
        var domain = email.GetEmailDomain();
        IDnsQueryResponse lookupResult;

        try
        {
            lookupResult = await _client.QueryAsync(domain, QueryType.ANY);
        }
        catch (Exception ex)
        {
            return new ValidationResult<DnsValidationResult>
            {
                Message = $"Unable to validate DNS due to an error: {ex.Message}",
                IsValid = false
            };
        }

        var aRecords = lookupResult.Answers.Where(x => x.RecordType == ResourceRecordType.A).ToList();
        var mxRecords = lookupResult.Answers.Where(x => x.RecordType == ResourceRecordType.MX).ToList();
        var allMxAndARecords = aRecords.Concat(mxRecords).ToList();

        var response = new ValidationResult<DnsValidationResult>
        {
            ValidationDetails = new DnsValidationResult
            {
                RecordsFound = allMxAndARecords
            }
        };

        if (!mxRecords.Any())
        {
            if (!aRecords.Any())
            {
                response.Message = "Domain contains no DNS responses for A records or MX records";
                response.IsValid = false;
                return response;
            }

            response.Message =
                "Domain contains A record(s) but no Mx Record. While email send might work it can not be guaranteed";
            if (_options.IsStrict)
            {
                response.IsValid = false;
                return response;
            }

            response.IsValid = true;
            return response;
        }

        response.Message = "Mx Record Exists";
        response.IsValid = true;
        return response;
    }
}