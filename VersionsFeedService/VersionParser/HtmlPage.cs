using HtmlAgilityPack;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using VersionsFeedService.VersionParser.Extensions;
using VersionsFeedService.VersionParser.Models;
using Version = VersionsFeedService.VersionParser.Architecture.Version;

[assembly: InternalsVisibleTo("VersionsFeedService.Test")]
namespace VersionsFeedService.VersionParser
{
    public class HtmlPage
    {
        private HtmlWeb Web { get; }

        private HtmlDocument Document { get; set; } = new();

        private string PageUri { get; set; } = string.Empty;

        private string BaseUri { get; set; } = "https://dotnet.microsoft.com";

        public string? VersionsFeedUri { get; } = "https://services.gingermintsoft.com/DotNetVersionFeed/Read/Versions";

        private string DownloadUri { get; } = "download/dotnet";

        private string DotNetUri { get; }

        private CultureInfo CultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("en-us");

        public HtmlPage()
        {
            Web = new HtmlWeb();
            DotNetUri= $"{BaseUri}/{CultureInfo.Name}/{DownloadUri}";
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        private HtmlPage(string baseUri)
        {
            Web = new HtmlWeb();

            BaseUri = baseUri;
            DotNetUri= $"{BaseUri}/{CultureInfo.Name}/{DownloadUri}";
        }

        /// <summary>
        /// Load Html page as content
        /// </summary>
        /// <param name="htmlPage">Load this page</param>
        /// <returns>Html document</returns>
        private async Task<HtmlDocument> LoadAsync(string htmlPage)
        {
            Document = htmlPage.Equals(PageUri)
                ? Document
                : await Web.LoadFromWebAsync(htmlPage)
                  ?? new HtmlDocument();

            PageUri = htmlPage;

            return Document;
        }

        /// <summary>
        /// Read download .NET versions at given page
        /// </summary>
        /// <param name="version">Search for this .NET version</param>
        /// <param name="architectures">Search for this architectures/bitness families</param>
        /// <returns>List of partially version download uris</returns>
        public async Task<List<string>> ReadDownloadPagesAsync(Version version, SdkArchitecture[] architectures)
        {
            var allDownLoads = new List<string>();

            foreach (var architecture in architectures)
            {
                // Get ARM bitness architecture for SDK
                var sdk = architecture == SdkArchitecture.Arm32
                    ? SdkArchitecture.Arm32.GetAttributeOfType<EnumMemberAttribute>().Value
                    : SdkArchitecture.Arm64.GetAttributeOfType<EnumMemberAttribute>().Value;

                // Get .NET main version
                var actual = version.GetAttributeOfType<EnumMemberAttribute>().Value;
                var htmlPage = await new HtmlPage().LoadAsync($"{DotNetUri}/{actual}");

                // Filter only for Linux .NET released SDKs
                var downLoads = htmlPage.DocumentNode
                    .SelectNodes($"//a[contains(text(), '{sdk}')]")
                    .Distinct()
                    .Select(row =>
                        row.GetAttributeValue("href", string.Empty))
                    .Where(href =>
                        href.Contains($"linux-{sdk!.ToLower()}-binaries") &&
                        !href.Contains("runtime") &&
                        !href.Contains("rc") &&
                        !href.Contains("preview"))
                    .ToList();

                // reverse version number ordering -> the actual is on top
                downLoads.Sort();
                downLoads.Reverse();

                for (var i = 0; i < downLoads.Count; i++)
                {
                    // build complete download uri
                    downLoads[i] = $"{BaseUri}{downLoads[i]}";
                }

                allDownLoads.AddRange(downLoads);
            }

            return allDownLoads;
        }

        /// <summary>
        /// Read actual download partial uri for .NET version and bitness
        /// </summary>
        /// <param name="version">Search for this .NET version</param>
        /// <param name="architectures">Search for this architectures/bitness families</param>
        /// <returns>Partial version download uri</returns>
        public async Task<string> ReadActualDownloadPageAsync(Version version, SdkArchitecture[] architectures)
        {
            var pages =  await ReadDownloadPagesAsync(version, architectures);

            return pages.First();
        }

        /// <summary>
        /// Read download partial uri for .NET version, bitness and a specific SDK version
        /// </summary>
        /// <param name="version">Search for this .NET version</param>
        /// <param name="specificVersion">Search for this .NET SDK version</param>
        /// <param name="architectures">Search for this architectures/bitness families</param>
        /// <returns>Partial version download uri</returns>
        public async Task<string> ReadDownloadPageForVersionAsync(Version version, string specificVersion, SdkArchitecture[] architectures)
        {
            var pages = await ReadDownloadPagesAsync(version, architectures);

            return pages.First(x => x.Contains(specificVersion));
        }

        /// <summary>
        /// Read .NET download uri with related checksum
        /// </summary>
        /// <param name="uri">Uri to download SDK page</param>
        /// <returns>Download SDK uri and checksum</returns>
        internal async Task<(string downLoadLink, string checkSum)> ReadDownloadUriAndChecksumAsync(string uri)
        {
            // load page content from uri
            var htmlPage = await new HtmlPage(BaseUri).LoadAsync($"{uri}");

            // .NET SDK download link and checksum
            var downLoadLink = htmlPage.DocumentNode
                .SelectNodes("//a[@id='directLink']")
                .Select(x => x.GetAttributeValue("href", string.Empty))
                .First();

            var checkSum = htmlPage.DocumentNode
                .SelectNodes("//input[@id='checksum']")
                .Select(x => x.GetAttributeValue("value", string.Empty))
                .First();

            return (downLoadLink, checkSum);
        }

        /// <summary>
        /// Read .NET download uri with related checksum
        /// </summary>
        /// <param name="uris">Uri array list pointing to the downloading SDK pages</param>
        /// <returns>Download SDK uri and checksum</returns>
        public async Task<(string downLoadLink, string checkSum)[]> ReadDownloadUriAndChecksumBulkAsync(IEnumerable<List<string>> uris)
        {
            var page = new HtmlPage();

            return await Task.WhenAll(uris
                .SelectMany(sdkArea => sdkArea)
                .Select(downloadPageLink => 
                    Task.Run(async () => 
                        await page.ReadDownloadUriAndChecksumAsync(downloadPageLink)))
                .AsParallel());
        }
    }
}