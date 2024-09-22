using AzureBlobWebAPIDemo.DTO;
using AzureBlobWebAPIDemo.Services.IServices;

namespace AzureBlobWebAPIDemo.Services
{
	public class ComputerVisionStuff : IComputerVisionStuff
	{
		public ComputerVisionSettings GetMeComputerVisionSettings()
		{
			var CVSettings = new ComputerVisionSettings();
			CVSettings.VISION_KEY = "";
			CVSettings.VISION_ENDPOINT = "";

			return CVSettings;
		}
	}
}
