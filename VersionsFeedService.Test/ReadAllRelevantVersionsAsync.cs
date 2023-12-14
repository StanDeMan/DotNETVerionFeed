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

            var architecture = new SdkArchitecture[] { SdkArchitecture.Arm32 };

            var downLoadLinks32 = await page.ReadDownloadPagesAsync(Version.Core8, architecture);
            Assert.IsNotNull(downLoadLinks32);
            
            architecture = new SdkArchitecture[] { SdkArchitecture.Arm64 };

            var downLoadLinks64 = await page.ReadDownloadPagesAsync(Version.Core8, architecture);
            Assert.IsNotNull(downLoadLinks64);

            downloadLinks.AddRange(downLoadLinks32);
            downloadLinks.AddRange(downLoadLinks64);

            architecture = new SdkArchitecture[] { SdkArchitecture.Arm32 };

            downLoadLinks32 = await page.ReadDownloadPagesAsync(Version.Core6, architecture);
            Assert.IsNotNull(downLoadLinks32);

            architecture = new SdkArchitecture[] { SdkArchitecture.Arm64 };

            downLoadLinks64 = await page.ReadDownloadPagesAsync(Version.Core6, architecture);
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

