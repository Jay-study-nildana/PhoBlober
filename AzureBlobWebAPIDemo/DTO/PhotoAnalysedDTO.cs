using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AzureBlobWebAPIDemo.DTO
{
	public class PhotoAnalysedDTO : PhotoUploadedDTO
	{
		public List<ImageTag> imageTags { set; get; }
	}
}
