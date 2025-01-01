using PhoBloberWebAPI.Services.IServices;

namespace PhoBloberWebAPI.Services
{
    public class BlobStorageStuff : IBlobStorageStuff
    {
        public string GiveMeAccessKeys(StorageSettingsService storageSettingsService)
        {
            var settings = storageSettingsService.GetStorageSettings();
            string AccessKeys = settings.AccessKeys;
            return AccessKeys;
        }

		public string GiveMeDefaultContainerName()
		{
            string DefaultContainerName = "phoblobercontainer1";
            return DefaultContainerName;
		}
	}
}
