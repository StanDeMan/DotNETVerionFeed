using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Models;
using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class AllRelevantVersionsAsync
    {
        [TestMethod]
        public async Task FindAllCoreVersionsTestMethodAsync()
        {
            var downloadLinks = new List<string>();

            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoadLinks32 = await page.ReadDownloadPagesAsync(Version.Core3, SdkArchitecture.Arm32);
            Assert.IsNotNull(downLoadLinks32);

            var downLoadLinks64 = await page.ReadDownloadPagesAsync(Version.Core3, SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoadLinks64);

            downloadLinks.AddRange(downLoadLinks32);
            downloadLinks.AddRange(downLoadLinks64);

            downLoadLinks32 = await page.ReadDownloadPagesAsync(Version.Core6, SdkArchitecture.Arm32);
            Assert.IsNotNull(downLoadLinks32);

            downLoadLinks64 = await page.ReadDownloadPagesAsync(Version.Core6, SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoadLinks64);

            downloadLinks.AddRange(downLoadLinks32);
            downloadLinks.AddRange(downLoadLinks64);

            foreach (var downLoad in downloadLinks)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }
    }
}

