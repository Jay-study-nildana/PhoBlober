using AzureBlogWebAppDemo.Services.IServices;

namespace AzureBlogWebAppDemo.Services
{
	public class BlobStorageStuff : IBlobStorageStuff
	{
		public string GiveMeAccessKeys()
		{
			return "not implemented";
		}

		public string GiveMeDefaultContainerName()
		{
			string DefaultContainerName = "containerdaabae5b-d87d-4c43-af04-b7a545d03851";
			return DefaultContainerName;
		}
	}
}
