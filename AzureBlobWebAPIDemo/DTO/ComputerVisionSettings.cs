
namespace AzureBlobWebAPIDemo.DTO
{
	public class ComputerVisionSettings
	{
		// Add your Computer Vision key and endpoint
		public string VISION_KEY { set; get; }//= Environment.GetEnvironmentVariable("VISION_KEY");
		public string VISION_ENDPOINT { set; get; } //= Environment.GetEnvironmentVariable("VISION_ENDPOINT");
	}
}
