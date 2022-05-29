using VersionsFeedService.VersionParser.Connection;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class CheckInternetConnection
    {
        [TestMethod]
        public async Task CheckTestAsync()
        {
            var ok = await Internet.CheckAsync();
            Assert.IsTrue(ok);
        }
    }
}
