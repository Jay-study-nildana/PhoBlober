namespace AzureBlobWebAPIDemo.DTO
{
    //https://learn.microsoft.com/en-us/dotnet/api/azure.storage.blobs.blobcontainerclient?view=azure-dotnet
    public class ContainerCreatedDTO
    {
        public string? StorageAccountName { set; get; }
        public string? ContainerName { set; get; }
        public string? DateTimeOfCreation { set; get; }

        public Uri? ContainerPrimaryUri { set; get; }
    }
}
