using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Models;
using VersionsFeedService.VersionParser.Transient;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class BulkReadAsync
    {
        private static SdkScrapingCatalog? _sdkScrapingCatalog;

        [TestMethod]
        public async Task BulkReadTestAsync()
        {
            var scrapeHtml = new HtmlPage();

            await using var catalogStream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("VersionsFeedService.Test.sdk-parser-catalog.json") 
                    ?? throw new ApplicationException();

            Assert.AreNotEqual(null, catalogStream);

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            _sdkScrapingCatalog =
                JsonConvert.DeserializeObject<SdkScrapingCatalog>(
                    await new StreamReader(catalogStream).ReadToEndAsync(),
                    jsonSerializerSettings);

            Assert.AreNotEqual(null, _sdkScrapingCatalog?.Sdks);

            var downloadPageLinks = await Task.WhenAll(_sdkScrapingCatalog!.Sdks.Select(sdk => Task.Run(() => 
                scrapeHtml.ReadDownloadPagesAsync(sdk.Version, sdk.Families))));

            var rawLinkCatalog = Array.Empty<(string downLoadLink, string checkSum)>();

            // test retry logic
            await new Retry<ArgumentNullException>().Policy.ExecuteAsync(async () =>
            {
                rawLinkCatalog = await HtmlPage.ReadDownloadUriAndChecksumBulkAsync(downloadPageLinks);
            });

            Assert.AreNotEqual(null, rawLinkCatalog);

            foreach (var (downLoadLink, checkSum) in rawLinkCatalog)
            {
                Debug.Print($"{downLoadLink}, {checkSum}");
            }
        }
    }
}
