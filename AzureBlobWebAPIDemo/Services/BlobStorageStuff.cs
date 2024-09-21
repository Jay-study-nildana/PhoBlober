using AzureBlobWebAPIDemo.Services.IServices;

namespace AzureBlobWebAPIDemo.Services
{
    public class BlobStorageStuff : IBlobStorageStuff
    {
        public string GiveMeAccessKeys()
        {
            string AccessKeys = "";
            return AccessKeys;
        }

		public string GiveMeDefaultContainerName()
		{
            string DefaultContainerName = "containeroneb291fa31-8fe4-4e05-a184-382cc7b4aa18";
            return DefaultContainerName;
		}
	}
}
