namespace AdvancedEmailValidator
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class BuildDependencies
    {
        private const string DisposableEmailDomainUri =
            "https://raw.githubusercontent.com/disposable-email-domains/disposable-email-domains/master/disposable_email_blocklist.conf";
        private static readonly string DisposableEmailFile = $"{Path.GetTempPath()}disposable_email_blocklist.conf";

        public async Task CheckDependencies()
        {
            await CheckDisposableFile();
        }

        private static async Task CheckDisposableFile()
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

        private static async Task DownloadDisposableEmailFile()
        {
            using var client = new HttpClient();
            using var result = await client.GetAsync(DisposableEmailDomainUri);
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var lines = content.Split(new [] { "\r\n", "\r", "\n"  }, StringSplitOptions.None);
                await File.WriteAllLinesAsync(DisposableEmailFile, lines);
            }
        }
    }
}
