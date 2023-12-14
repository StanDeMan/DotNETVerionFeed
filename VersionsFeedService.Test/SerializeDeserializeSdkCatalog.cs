using Newtonsoft.Json;
using VersionsFeedService.VersionParser.Models;
using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.Test
{
    [TestClass]
    public class SerializeDeserializeSdkCatalog
    {
        [TestMethod]
        public void SerializeDeserializeSdkCatalogTest()
        {
            Console.WriteLine();
            Console.WriteLine("Sdk scraping preset part:");

            var sdkPresets = new List<SdkScraperPreset>
            {
                new()
                {
                    Version = Version.Core6,
                    Families = new[] { SdkArchitecture.Arm32, SdkArchitecture.Arm64 }
                },
                new()
                {
                    Version = Version.Core7,
                    Families = new[] { SdkArchitecture.Arm32, SdkArchitecture.Arm64 }
                },
                new()
                {
                    Version = Version.Core8,
                    Families = new[] { SdkArchitecture.Arm32, SdkArchitecture.Arm64 }
                }
            };

            var catalog = new SdkScrapingCatalog
            {
                Culture = "en-Us",
                MicrosoftBaseUri = "https://dotnet.microsoft.com",
                SdksPresets = sdkPresets
            };

            // serialize to json
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            var jsonCatalog = JsonConvert.SerializeObject(catalog, Formatting.Indented, jsonSerializerSettings);
            Assert.IsNotNull(jsonCatalog);

            // and back into the object
            var readCatalog = JsonConvert.DeserializeObject<SdkScrapingCatalog>(jsonCatalog, jsonSerializerSettings);
            Assert.IsNotNull(readCatalog);

            Console.WriteLine($"Culture: {readCatalog.Culture}");
            Console.WriteLine($"BaseUri: {readCatalog.MicrosoftBaseUri}\r\n");

            var sdks = readCatalog.Sdks;

            foreach (var sdk in sdks)
            {
                Console.WriteLine($"Sdk: {sdk.Family}, Version: {sdk.Version}");
            }

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var directory = Path.GetDirectoryName(path);
            Assert.IsNotNull(directory);

            Console.WriteLine();
            Console.WriteLine("Catalog:");
            Console.WriteLine($"{jsonCatalog}");
        }
    }
}
