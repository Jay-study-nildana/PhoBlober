using System.ComponentModel.DataAnnotations;

namespace AzureBlogWebAppDemo.DTO
{
    public class PhotoUploadMultipleDTO
    {
        public string? PhotoDescription { get; set; }
        public string? PhotoName { get; set; }
        public string? containerName { get; set; }
        [Required]
        public List<IFormFile>? Image { get; set; }
    }
}
