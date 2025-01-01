using PhoBloberWebApp.Services.IServices;

namespace PhoBloberWebApp.Services
{
	public class BlobStorageStuff : IBlobStorageStuff
	{
		public string GiveMeAccessKeys()
		{
			return "not implemented";
		}

		public string GiveMeDefaultContainerName()
		{
			string DefaultContainerName = "phoblobercontainer1";
			return DefaultContainerName;
		}
	}
}
