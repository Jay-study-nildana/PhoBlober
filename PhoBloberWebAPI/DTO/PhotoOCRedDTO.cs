using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace PhoBloberWebAPI.DTO
{
    public class PhotoOCRedDTO : PhotoUploadedDTO
    {
        public List<string> foundlines { set; get; }
    }
}
