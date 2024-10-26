using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Extensions;
using VersionsFeedService.VersionParser.Models;
using VersionsFeedService.VersionParser.Sdk;
using VersionsFeedService.VersionParser.Transient;

namespace VersionsFeedService
{
    public class VersionsFeedWorker : BackgroundService
    {
        private readonly ILogger<VersionsFeedWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        private const string SdkCatalogKey = "SdkCatalogKey";
        private readonly IMemoryCache _cache;
        private static SdkCatalog? _cachedSdkCatalog;
        private static SdkScrapingCatalog? _cachedSdkScrapingCatalog;

        public VersionsFeedWorker(
            ILogger<VersionsFeedWorker> logger, 
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _logger = logger;
            _serviceProvider = serviceProvider;

            if (_cache.TryGetValue(SdkCatalogKey, out _cachedSdkCatalog)) return;

            _cachedSdkCatalog = new SdkCatalog();
            _cache.Set(SdkCatalogKey, _cachedSdkCatalog, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1))
                .SetAbsoluteExpiration(TimeSpan.FromDays(2)));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UpdateVersions();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromHours(12), cancellationToken);
            }
        }

        private async Task UpdateVersions()
        {
            using var scope = _serviceProvider.CreateScope();

            try
            {
                await using var catalogStream = Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream("VersionsFeedService.sdk-parser-catalog.json") 
                        ?? throw new ApplicationException();
                
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                _cachedSdkScrapingCatalog =
                    JsonConvert.DeserializeObject<SdkScrapingCatalog>(
                        await new StreamReader(catalogStream).ReadToEndAsync(),
                        jsonSerializerSettings);

                if (_cachedSdkScrapingCatalog?.Sdks == null) throw new ApplicationException();

                var rawLinkCatalog = Array.Empty<(string downLoadLink, string checkSum)>();
                var scrapeHtml = new HtmlPage();
                var downloadPageLinks = new List<string>[] { };

                await new Retry<ArgumentNullException>().Policy.ExecuteAsync(async () =>
                {
                    downloadPageLinks = await Task.WhenAll(_cachedSdkScrapingCatalog.Sdks.Select(sdk =>
                        Task.Run(() => scrapeHtml.ReadDownloadPagesAsync(sdk.Version, sdk.Families))));
                });

                await new Retry<ArgumentNullException>().Policy.ExecuteAsync(async () =>
                {
                    rawLinkCatalog = await scrapeHtml.ReadDownloadUriAndChecksumBulkAsync(downloadPageLinks);
                });

                var ok = FillCachedSdkCatalog(rawLinkCatalog);

                _cache.Remove(SdkCatalogKey);
                _cache.Set(SdkCatalogKey, ok 
                    ? _cachedSdkCatalog 
                    : new SdkCatalog());
            }
            catch (Exception e)
            {
                _logger.LogError($"VersionFeedWorker.UpdateVersions error: {e}. Scope: {scope}");
            }
        }

        /// <summary>
        /// Fill the _cachedSdkCatalog for selected raspberry pi
        /// Only filled once for selected raspberry pi
        /// -> than no need to load again...
        /// </summary>
        /// <param name="rawLinkCatalog">Raw catalog data: (link, checkSum)</param>
        private static bool FillCachedSdkCatalog(IEnumerable<(string, string)> rawLinkCatalog)
        {
            _cachedSdkCatalog = new SdkCatalog();                                       // reset entries

            foreach (var (downLoadLink, checkSum) in rawLinkCatalog)
            {
                var dotNetPart = downLoadLink.Split('/')[7].Split('-');                 // read .NET part from download uri

                // extract items and fill cached catalog with new entries
                _cachedSdkCatalog?.Items.Add(new SdkCatalogItem(
                    $"{dotNetPart[2].Split('.')[0]}.{dotNetPart[2].Split('.')[1]}",     // SDK version
                    $"{dotNetPart[2]}",                                                 // SDK release number
                    dotNetPart[4].Contains(Platform.Bitness64.ToMemberString())         // read SDK architecture    
                        ? SdkArchitecture.Arm64.ToMemberString()                                                     
                        : SdkArchitecture.Arm32.ToMemberString(),
                    downLoadLink,
                    checkSum));
            }

            return _cachedSdkCatalog != null && _cachedSdkCatalog.Items.Any();
        }
    }
}