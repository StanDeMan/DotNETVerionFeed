using DotNETVersionFeed.VersionParser.Architecture;
using Version = DotNETVersionFeed.VersionParser.Architecture.Version;

namespace DotNETVersionFeed.VersionParser.Models
{
    public class SdkScraper
    {
        public Version Version { get; set; }

        public Sdk Family { get; set; }
    }
}