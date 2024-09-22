using AzureBlobWebAPIDemo.DTO;
using AzureBlobWebAPIDemo.Services.IServices;

namespace AzureBlobWebAPIDemo.Services
{
    public class TranslatorStuff : ITranslatorStuff
    {
        public TranslatorSettings GetMeTranslatorSettings()
        {
            TranslatorSettings translatorSettings = new TranslatorSettings();
            translatorSettings.key = "";
            translatorSettings.endpoint = "";

            return translatorSettings;
        }
    }
}
