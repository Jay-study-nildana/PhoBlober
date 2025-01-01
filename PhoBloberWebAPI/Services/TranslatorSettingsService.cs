using Microsoft.Extensions.Options;
using PhoBloberWebAPI.Settings;

namespace PhoBloberWebAPI.Services
{
    //TODO : Make these services implement an interface
    public class TranslatorSettingsService
    {
        private readonly TranslatorSettings _translatorSettings;

        public TranslatorSettingsService(IOptions<TranslatorSettings> translatorSettings)
        {
            _translatorSettings = translatorSettings.Value;
        }

        public TranslatorSettings GetMeTranslatorSettings()
        {
            return _translatorSettings;
        }
    }
}
