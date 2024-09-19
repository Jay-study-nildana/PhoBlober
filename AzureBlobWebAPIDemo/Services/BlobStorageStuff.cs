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
            string DefaultContainerName = "";
            return DefaultContainerName;
		}
	}
}
