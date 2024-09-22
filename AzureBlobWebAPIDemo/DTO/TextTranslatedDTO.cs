namespace AzureBlobWebAPIDemo.DTO
{
    public class TextTranslatedDTO : TextDTO
    {
        public TextTranslatedDTO() {
            TextUnits = new List<TranslatedTextUnit>();
        }
        public List<TranslatedTextUnit>? TextUnits { get; set; }
    }

    public class TranslatedTextUnit
    {
        public string? Language { get; set; }
        public string? TranslatedText { get; set; }
    }
}
