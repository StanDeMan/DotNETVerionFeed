using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VersionsFeedService.VersionParser.Sdk;

namespace DotNETVersionFeed.Controllers
{
    [ApiController]
    [Route("DotNetVersionFeed")]
    public class VersionsController : Controller
    {
        private const string SdkCatalogKey = "SdkCatalogKey";

        private readonly ILogger<VersionsController> _logger;

        private readonly IMemoryCache _cache;

        public VersionsController(
            IMemoryCache cache,
            ILogger<VersionsController> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        [HttpGet]
        [Route("Read/Versions")]
        public SdkCatalog? Get()
        {
            try
            {
                return _cache.Get<SdkCatalog>(SdkCatalogKey);
            }
            catch (Exception e)
            {
                _logger.LogError($"VersionController.GET error: {e}");
            }

            return new SdkCatalog();
        }
    }
}
