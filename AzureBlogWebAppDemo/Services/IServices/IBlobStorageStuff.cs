namespace AzureBlogWebAppDemo.Services.IServices
{
    public interface IBlobStorageStuff
    {
		//Not the most ideal solution. but we will make this better later.
		public string GiveMeAccessKeys();
		public string GiveMeDefaultContainerName();
	}
}
