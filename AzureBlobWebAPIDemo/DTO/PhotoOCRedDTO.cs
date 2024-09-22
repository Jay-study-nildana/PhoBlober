using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AzureBlobWebAPIDemo.DTO
{
    public class PhotoOCRedDTO : PhotoUploadedDTO
    {
        public List<string> foundlines { set; get; }
    }
}
