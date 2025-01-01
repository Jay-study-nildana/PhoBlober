using PhoBloberWebAPI.DTO;

namespace PhoBloberWebAPI.Services.IServices
{
	public interface IComputerVisionStuff
	{
		public ComputerVisionSettings GetMeComputerVisionSettings(CVSettingsService cVSettingsService);
	}
}
