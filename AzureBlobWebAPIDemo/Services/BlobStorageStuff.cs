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
            string DefaultContainerName = "containerdaabae5b-d87d-4c43-af04-b7a545d03851";
            return DefaultContainerName;
		}
	}
}
