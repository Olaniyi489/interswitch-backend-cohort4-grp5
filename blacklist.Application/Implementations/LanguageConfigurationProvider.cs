namespace blacklist.Application.Implementations
{

    public class LanguageConfigurationProvider : ILanguageConfigurationProvider
    {
        private readonly LanguageSettings _settings;
        private const string FILE_EXTENSION = ".json";
        private readonly IDictionary<string, LanguagePack> _packs;
        private readonly ILogger _logger;
        private readonly LanguagePackagePathHelper _languagePackage;
        public LanguageConfigurationProvider(IOptions<LanguageSettings> settingsProvider,
            ILogger<LanguageConfigurationProvider> logger,
            LanguagePackagePathHelper languagePackage)
        {
            _settings = settingsProvider.Value ?? throw new ArgumentNullException(nameof(settingsProvider));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _packs = new Dictionary<string, LanguagePack>(_settings.Bundles.Length);          
            _languagePackage = languagePackage ?? throw new ArgumentNullException(nameof(languagePackage));
            CreateLanguagePacks();
        }

        private void CreateLanguagePacks()
        {
            var serializer = JsonSerializer.CreateDefault();
            foreach (var bundle in _settings.Bundles)
            {
                var pack = new LanguagePack(bundle.DefaultMessage);
                var location = $"{_languagePackage.GetLanguagePackagePath()}{Path.DirectorySeparatorChar}{bundle.LanguageCode}{FILE_EXTENSION}";
                if (!File.Exists(location))
                {
                    location = $"{_settings.BaseLocation}{Path.DirectorySeparatorChar}{bundle.LanguageCode}{FILE_EXTENSION}";
                    if (!File.Exists(location))
                    {
                        _logger.LogWarning("No bundle file found for language - {0}. file location - {1}",
                            new[] { bundle.LanguageCode, location });
                        continue;
                    }
                }
                var reader = File.OpenText(location);
                if(reader is null)
                {
                    location = $"{_settings.BaseLocation}{Path.DirectorySeparatorChar}{bundle.LanguageCode}{FILE_EXTENSION}";
                    if (!File.Exists(location))
                    {
                        _logger.LogWarning("No bundle file found for language - {0}. file location - {1}",
                            new[] { bundle.LanguageCode, location });
                        continue;
                    }
                    reader = File.OpenText(location);
                }
                pack.Mappings = (Dictionary<string, string>)serializer.Deserialize(reader, typeof(Dictionary<string, string>));
                _packs[bundle.LanguageCode] = pack;
            }
        }

        public LanguagePack GetPack(string language)
        {
            return (_packs.TryGetValue(language, out var pack)) ? pack : null;
        }
    }

}
