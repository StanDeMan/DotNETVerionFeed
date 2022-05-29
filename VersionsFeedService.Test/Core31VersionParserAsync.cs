﻿using VersionsFeedService.VersionParser;
using VersionsFeedService.VersionParser.Models;
using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class Core31VersionParserAsync
    {
        [TestMethod]
        public async Task FindCore31Arm64TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core3, SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task FindCore31Arm32TestMethodAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoads = await page.ReadDownloadPagesAsync(Version.Core3, SdkArchitecture.Arm32);
            Assert.IsNotNull(downLoads);

            foreach (var downLoad in downLoads)
            {
                Console.WriteLine($"{downLoad} \r\n");
            }
        }

        [TestMethod]
        public async Task ReadActualCore3Async()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoad = await page.ReadActualDownloadPageAsync(Version.Core3, SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            downLoad = await page.ReadActualDownloadPageAsync(Version.Core3, SdkArchitecture.Arm32);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }

        [TestMethod]
        public async Task ReadCore3VersionAsync()
        {
            var page = new HtmlPage();
            Assert.IsNotNull(page);

            var downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core3, "3.1.302", SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");

            downLoad = await page.ReadDownloadPageForVersionAsync(Version.Core3, "3.1.300", SdkArchitecture.Arm64);
            Assert.IsNotNull(downLoad);

            Console.WriteLine($"{downLoad} \r\n");
        }
    }
}

