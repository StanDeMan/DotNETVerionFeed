using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Models;
using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class Core7VersionParserAsync
    {
        [TestMethod]
        public async Task FindCore7Arm64TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core7, SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task FindCore7Arm32TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core7, SdkArchitecture.Arm32);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task ReadActualCore7Async()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoad = await page.ReadActualDownloadPageAsync(Version.Core7, SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            downLoad = await page.ReadActualDownloadPageAsync(Version.Core7, SdkArchitecture.Arm32);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }

        [TestMethod]
        public async Task ReadCore7VersionAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core7, "7.0.100", SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core7, "7.0.100", SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }
    }
}

