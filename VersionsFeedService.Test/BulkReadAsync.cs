using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Models;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class BulkReadAsync
    {
        private static SdkScrapingCatalog? _cachedSdkScrapingCatalog;

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

            _cachedSdkScrapingCatalog =
                JsonConvert.DeserializeObject<SdkScrapingCatalog>(
                    await new StreamReader(catalogStream).ReadToEndAsync(),
                    jsonSerializerSettings);

            Assert.AreNotEqual(null, _cachedSdkScrapingCatalog?.Sdks);

            var downloadPageLinks = await Task.WhenAll(_cachedSdkScrapingCatalog!.Sdks.Select(sdk => Task.Run(() => 
                scrapeHtml.ReadDownloadPagesAsync(sdk.Version, sdk.Family))));

            var rawLinkCatalog = await HtmlPage.ReadDownloadUriAndChecksumBulkAsync(downloadPageLinks);

            foreach (var catalogItem in rawLinkCatalog)
            {
                Debug.Print($"{catalogItem.Item1}, {catalogItem.Item2}");
            }
        }
    }
}
