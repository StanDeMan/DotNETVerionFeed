using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.VersionParser.Models
{
    public class SdkScraperPreset
    {
        public Version Version { get; set; }

        public required SdkArchitecture[] Families { get; set; }
    }
}
