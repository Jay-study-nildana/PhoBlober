using System.ComponentModel.DataAnnotations;

namespace AzureBlobWebAPIDemo.DTO
{
    public class PhotoUploadDTO
    {
        public string? PhotoDescription { get; set; }
        public string? PhotoName { get; set; }

        public string? containerName { get; set; }
        [Required]
        public IFormFile? Image { get; set; }
    }
}
