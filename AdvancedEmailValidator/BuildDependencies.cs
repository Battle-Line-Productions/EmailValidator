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
// File: BuildDependencies.cs
// ---------------------------------------------------------------------------

#endregion

#region Usings

using AdvancedEmailValidator.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

#endregion

namespace AdvancedEmailValidator;

public class BuildDependencies : IBuildDependencies
{
    private const string DisposableEmailDomainUri =
        "https://raw.githubusercontent.com/disposable-email-domains/disposable-email-domains/master/disposable_email_blocklist.conf";

    private static readonly string DisposableEmailFile = $"{Path.GetTempPath()}disposable_email_blocklist.conf";

    private readonly IHttpClientFactory _httpClientFactory;

    public BuildDependencies(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task CheckDependencies()
    {
        await CheckDisposableFile();
    }

    private async Task CheckDisposableFile()
    {
        if (!File.Exists(DisposableEmailFile))
        {
            await DownloadDisposableEmailFile();
            return;
        }

        var threshold = DateTime.UtcNow.AddDays(-1);
        if (File.GetLastWriteTimeUtc(DisposableEmailFile) > threshold)
        {
            await DownloadDisposableEmailFile();
        }
    }

    private async Task DownloadDisposableEmailFile()
    {
        var client = _httpClientFactory.CreateClient();

        HttpResponseMessage result;

        try
        {
            result = await client.GetAsync(DisposableEmailDomainUri);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to download disposable email file", ex);
        }

        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            try
            {
                await File.WriteAllLinesAsync(DisposableEmailFile, lines);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to write disposable email file to disk", ex);
            }
        }
    }
}