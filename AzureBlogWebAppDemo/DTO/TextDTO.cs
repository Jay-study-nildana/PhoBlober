using System.ComponentModel.DataAnnotations;

namespace AzureBlogWebAppDemo.DTO
{
    public class TextDTO
    {
        [Required]
        public string? OriginalText { get; set; }
        public string? AdditionalInfo { get; set; }
        public List<string>? TargetLanguages { get; set; }
    }
}
