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
			string DefaultContainerName = "";
			return DefaultContainerName;
		}
	}
}
