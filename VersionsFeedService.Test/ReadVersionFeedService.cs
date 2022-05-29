using System.Diagnostics;
using VersionsFeedService.VersionParser.Web;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class ReadVersionFeedService
    {
        [TestMethod]
        public async Task ReadVersionFeedServiceAsync()
        {
            const string uri = @"https://dotnetversionfeed.azurewebsites.net/versions";

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var ret = await new Request().ReadVersionFeedService(uri);

            stopwatch.Stop();
            Assert.IsNotNull(ret);
            Debug.Print($"Milliseconds: {stopwatch.ElapsedMilliseconds}");
            Debug.Print(ret);
        }

        [TestMethod]
        public async Task ReadVersionFeedServiceWithoutUriAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var ret = await new Request().ReadVersionFeedService("");

            stopwatch.Stop();
            Assert.IsNotNull(ret);
            Debug.Print($"Milliseconds: {stopwatch.ElapsedMilliseconds}");
            Debug.Print(ret);

            stopwatch.Start();

            ret = await new Request().ReadVersionFeedService();

            stopwatch.Stop();
            Assert.IsNotNull(ret);
            Debug.Print($"Milliseconds: {stopwatch.ElapsedMilliseconds}");
            Debug.Print(ret);
        }
    }
}
