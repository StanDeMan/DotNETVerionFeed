using System.Reflection;
using System.Runtime.Serialization;
using DotNETVersionFeed.VersionParser;
using DotNETVersionFeed.VersionParser.Extensions;
using DotNETVersionFeed.VersionParser.Models;
using DotNETVersionFeed.VersionParser.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Platform = DotNETVersionFeed.VersionParser.Models.Platform;

namespace DotNETVersionFeed.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VersionController : Controller
    {
        private const string SdkCatalogKey = "SdkCatalogKey";
        private static SdkCatalog _cachedSdkCatalog = null!;
        private static SdkScrapingCatalog? _cachedSdkScrapingCatalog;

        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<VersionController> _logger;
        private readonly IMemoryCache _cache;

        public VersionController(ILogger<VersionController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;

            if (_cache.TryGetValue(SdkCatalogKey, out _cachedSdkCatalog)) return;

            _cachedSdkCatalog = new SdkCatalog();
            _cache.Set(SdkCatalogKey, _cachedSdkCatalog, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(12))
                .SetAbsoluteExpiration(TimeSpan.FromDays(1)));
        }

        [HttpGet(Name = "Version")]
        public async Task<SdkCatalog?> Get()
        {
            if (_cachedSdkCatalog.Items.Any()) return _cachedSdkCatalog;

            try
            {
                await using var catalogStream = Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream("DotNETVersionFeed.sdk-parser-catalog.json");

                if (catalogStream == null) throw new IndexOutOfRangeException();

                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                _cachedSdkScrapingCatalog = JsonConvert.DeserializeObject<SdkScrapingCatalog>(
                    await new StreamReader(catalogStream).ReadToEndAsync(), 
                    jsonSerializerSettings);

                if (_cachedSdkScrapingCatalog?.Sdks == null) throw new IndexOutOfRangeException();

                var scrapeHtml = new HtmlPage();

                var downloadPageLinks = await Task.WhenAll(_cachedSdkScrapingCatalog.Sdks.Select(sdk => Task.Run(() =>
                    scrapeHtml.ReadDownloadPagesAsync(sdk.Version, sdk.Family))));

                var rawLinkCatalog = await scrapeHtml.ReadDownloadUriAndChecksumBulkAsync(downloadPageLinks);

                var ok = FillCachedSdkCatalog(rawLinkCatalog);

                _cache.Set(SdkCatalogKey, ok 
                    ? _cachedSdkCatalog 
                    : new SdkCatalog());
            }
            catch (Exception e)
            {
                _logger.LogError($"GET/version: {e}");
            }

            return _cache.Get<SdkCatalog>(SdkCatalogKey);
        }

        /// <summary>
        /// Fill the _cachedSdkCatalog for selected raspberry pi
        /// Only filled once for selected raspberry pi
        /// -> than no need to load again...
        /// </summary>
        /// <param name="rawLinkCatalog">Raw catalog data: (link, checkSum)</param>
        private static bool FillCachedSdkCatalog(IEnumerable<(string, string)> rawLinkCatalog)
        {
            foreach (var (downLoadLink, checkSum) in rawLinkCatalog)
            {
                var dotNetPart = downLoadLink.Split('/')[7].Split('-');                 // read .NET part from download uri

                // fill to cached catalog
                _cachedSdkCatalog?.Items.Add(new SdkCatalogItem(
                    $"{dotNetPart[2].Split('.')[0]}.{dotNetPart[2].Split('.')[1]}",     // extract belonging SDK version
                    dotNetPart[4].Contains(Platform.Bitness64.GetAttributeOfType<EnumMemberAttribute>().Value ?? "64")         // read SDK architecture    
                        ? SdkArchitecture.Arm64                                                     
                        : SdkArchitecture.Arm32,
                    downLoadLink,
                    checkSum));
            }

            return _cachedSdkCatalog != null && _cachedSdkCatalog.Items.Any();
        }
    }
}
