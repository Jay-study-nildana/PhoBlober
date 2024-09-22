using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AzureBlogWebAppDemo.DTO
{
    public class PhotoOCRedDTO : PhotoUploadDTO
    {
        public List<string> foundlines { set; get; }
    }
}
