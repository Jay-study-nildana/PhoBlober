using Microsoft.Extensions.Options;
using PhoBloberWebAPI.Settings;

namespace PhoBloberWebAPI.Services
{
    //TODO : Make these services implement an interface
    public class CVSettingsService
    {
        private readonly CVSettings _cvSettings;

        public CVSettingsService(IOptions<CVSettings> cvSettings)
        {
            _cvSettings = cvSettings.Value;
        }

        public CVSettings GetCVSettings()
        {
            return _cvSettings;
        }
    }
}
