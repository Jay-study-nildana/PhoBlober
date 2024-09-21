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
            string DefaultContainerName = "containerabd2b812-c7db-4a61-b8f3-a8181baea1d2";
            return DefaultContainerName;
		}
	}
}
