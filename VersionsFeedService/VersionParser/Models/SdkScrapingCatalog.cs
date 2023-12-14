namespace VersionsFeedService.VersionParser.Models
{
    public class SdkScrapingCatalog
    {
        private List<SdkScraperPreset>? _sdksPresets;

        public string MicrosoftBaseUri { get; set; } = string.Empty;

        public string Culture { get; set; } = "en-US";

        public List<SdkScraper> Sdks { get; set; }

        public List<SdkScraperPreset>? SdksPresets
    {
            get => _sdksPresets;

            set
            {
                _sdksPresets = value;

                foreach (var preset in _sdksPresets!.ToList())
                {
                    foreach (var family in preset.Families)
                    {
                        Sdks.Add(new SdkScraper()
                        {
                            Version = preset.Version,
                            Family = family
                        });
                    }
                }
            }
        }


        public SdkScrapingCatalog()
        {
            Sdks = new List<SdkScraper>();
            _sdksPresets = new List<SdkScraperPreset>();
        }
    }
}
