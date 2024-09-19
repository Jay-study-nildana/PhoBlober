namespace AzureBlobWebAPIDemo.DTO
{
    public class PhotoUploadedDTO 
    {
        public string? PhotoDescription { get; set; }
        public string? PhotoName { get; set; }

        public string? AccountName { get; set; }

        public string? BlobContainerName { get; set; }

        public string? BlobName { get; set; }

        public Uri BlobUri { get; set; }
    }
}
