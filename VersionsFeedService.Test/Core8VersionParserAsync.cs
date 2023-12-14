using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Models;
using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class Core8VersionParserAsync
    {
        [TestMethod]
        public async Task FindCore8Arm64TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new SdkArchitecture[] { SdkArchitecture.Arm64 };

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core8, architecture);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task FindCore8Arm32TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new SdkArchitecture[] { SdkArchitecture.Arm32 };

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core8, architecture);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task ReadActualCore8Async()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new SdkArchitecture[] { SdkArchitecture.Arm64 };

            var downLoad = await page.ReadActualDownloadPageAsync(Version.Core8, architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            architecture = new SdkArchitecture[] { SdkArchitecture.Arm32 };

            downLoad = await page.ReadActualDownloadPageAsync(Version.Core8, architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }

        [TestMethod]
        public async Task ReadCore8VersionAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new SdkArchitecture[] { SdkArchitecture.Arm32 };

            var downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core8, "8.0.100", architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            architecture = new SdkArchitecture[] { SdkArchitecture.Arm64 };

            downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core8, "8.0.100", architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }
    }
}

