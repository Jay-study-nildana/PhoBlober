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
			string DefaultContainerName = "containeroneb291fa31-8fe4-4e05-a184-382cc7b4aa18";
			return DefaultContainerName;
		}
	}
}
