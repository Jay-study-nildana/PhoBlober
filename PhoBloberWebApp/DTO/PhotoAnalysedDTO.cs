using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace PhoBloberWebApp.DTO
{
	public class PhotoAnalysedDTO : PhotoUploadDTO
	{
		public List<ImageTag> imageTags { set; get; }
	}
}
