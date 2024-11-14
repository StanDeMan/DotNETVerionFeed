using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Models;
using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class Core9VersionParserAsync
    {
        [TestMethod]
        public async Task FindCore9Arm64TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new[] { SdkArchitecture.Arm64 };

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core9, architecture);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task FindCore9Arm32TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new[] { SdkArchitecture.Arm32 };

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core9, architecture);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task ReadActualCore9Async()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new[] { SdkArchitecture.Arm64 };

            var downLoad = await page.ReadActualDownloadPageAsync(Version.Core9, architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            architecture = new[] { SdkArchitecture.Arm32 };

            downLoad = await page.ReadActualDownloadPageAsync(Version.Core9, architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }

        [TestMethod]
        public async Task ReadCore9VersionAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var architecture = new[] { SdkArchitecture.Arm32 };

            var downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core9, "9.0.100", architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            architecture = new[] { SdkArchitecture.Arm64 };

            downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core9, "9.0.100", architecture);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }
    }
}

