namespace AzureBlobWebAPIDemo.DTO
{
    public class GetAllBlobsDTO
    {
        public List<string> BlobFullURL { get; set; }
        public int blobCount { get; set; }

        public GetAllBlobsDTO()
        {
            BlobFullURL = new List<string>();
            blobCount = 0;
        }
    }
}
