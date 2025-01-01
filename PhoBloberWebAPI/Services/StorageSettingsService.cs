using Microsoft.Extensions.Options;
using PhoBloberWebAPI.Settings;

namespace PhoBloberWebAPI.Services
{
    //TODO : Make these services implement an interface
    public class StorageSettingsService
    {
        private readonly StorageSettings _storageSettings;

        public StorageSettingsService(IOptions<StorageSettings> storageSettings)
        {
            _storageSettings = storageSettings.Value;
        }

        public StorageSettings GetStorageSettings()
        {
            return _storageSettings;
        }
    }

}
