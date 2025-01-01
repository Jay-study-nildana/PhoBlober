using PhoBloberWebAPI.DTO;
using PhoBloberWebAPI.Services.IServices;

namespace PhoBloberWebAPI.Services
{
    public class TranslatorStuff : ITranslatorStuff
    {
        public TranslatorSettings GetMeTranslatorSettings(TranslatorSettingsService translatorSettingsService)
        {
            var settings = translatorSettingsService.GetMeTranslatorSettings();

            TranslatorSettings translatorSettings = new TranslatorSettings();
            translatorSettings.key = settings.Key;
            translatorSettings.endpoint = settings.Endpoint;

            return translatorSettings;
        }
    }
}
