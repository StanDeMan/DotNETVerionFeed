﻿using Version = VersionsFeedService.VersionParser.Architecture.Version;

namespace VersionsFeedService.VersionParser.Models
{
    public class SdkScraper
    {
        public Version Version { get; set; }

        public required SdkArchitecture[] Families { get; set; }
    }
}