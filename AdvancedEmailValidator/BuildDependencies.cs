using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvancedEmailValidator.Interfaces;

public class BuildDependencies : IBuildDependencies
{
    private const string DisposableEmailDomainUri =
        "https://raw.githubusercontent.com/disposable-email-domains/disposable-email-domains/master/disposable_email_blocklist.conf";

    private const string DisposableEmailClientName = "DisposableEmailClient";

    private static readonly string DisposableEmailFile = $"{Path.GetTempPath()}disposable_email_blocklist.conf";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);  // Initial count and maximum count of 1.

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
        await _semaphore.WaitAsync();  // Wait until it's safe to enter.

        try
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
        finally
        {
            _semaphore.Release();  // Release the semaphore to allow other threads to enter.
        }
    }

    private async Task DownloadDisposableEmailFile()
    {
        var client = _httpClientFactory.CreateClient(DisposableEmailClientName);

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
