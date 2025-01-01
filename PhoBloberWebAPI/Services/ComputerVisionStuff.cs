using PhoBloberWebAPI.DTO;
using PhoBloberWebAPI.Services.IServices;

namespace PhoBloberWebAPI.Services
{
	public class ComputerVisionStuff : IComputerVisionStuff
	{
		public ComputerVisionSettings GetMeComputerVisionSettings(CVSettingsService cVSettingsService)
		{
			var settings = cVSettingsService.GetCVSettings();
			var CVSettings = new ComputerVisionSettings();
			CVSettings.VISION_KEY = settings.VISION_KEY;
			CVSettings.VISION_ENDPOINT = settings.VISION_ENDPOINT;

			return CVSettings;
		}
	}
}
