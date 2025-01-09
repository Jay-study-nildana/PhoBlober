using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PhoBloberWebAPI.DTO;
using PhoBloberWebAPI.Services;
using PhoBloberWebAPI.Services.IServices;

namespace PhoBloberWebAPI.Utilities
{
    public class BlobUtilities
    {
        public async Task<ContainerCreatedDTO> CreateBlobContainerAsync(string containerName, IBlobStorageStuff _blobStorageStuff,
            StorageSettingsService _storageSettingsService)
        {
            // Get storage access keys and create storage connection string
            var storageAccessKeys = _blobStorageStuff.GiveMeAccessKeys(_storageSettingsService);
            string? storageConnectionString = storageAccessKeys;

            // Create a BlobServiceClient using the connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

            // Prepare and return the ContainerCreatedDTO
            return new ContainerCreatedDTO
            {
                ContainerName = containerName,
                DateTimeOfCreation = DateTime.UtcNow.ToString(),
                ContainerPrimaryUri = containerClient.Uri,
                StorageAccountName = containerClient.AccountName
            };
        }

        public GetAllContainersDTO GetAllContainers(BlobServiceClient blobServiceClient)
        {
            var containerList = blobServiceClient.GetBlobContainers();
            GetAllContainersDTO getAllContainersDTO = new GetAllContainersDTO();

            foreach (var container in containerList)
            {
                var containerName = container.Name;
                getAllContainersDTO.ContainerIds.Add(containerName);
                getAllContainersDTO.ContainerCount++;
            }

            return getAllContainersDTO;
        }

        public ContainerCreatedDTO ToggleContainerPublic(string containerName, IBlobStorageStuff _blobStorageStuff,
            StorageSettingsService _storageSettingsService)
        {
            var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys(_storageSettingsService);

            string? storageConnectionString = storageaccesskeys;

            // Create a client that can authenticate with a connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

            // Create the container and return a container client object
            //BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var response = containerClient.SetAccessPolicy(PublicAccessType.BlobContainer);

            var containerDTO = new ContainerCreatedDTO();
            containerDTO.ContainerName = containerName;
            containerDTO.DateTimeOfCreation = DateTime.UtcNow.ToString();
            containerDTO.ContainerPrimaryUri = containerClient.Uri;
            containerDTO.StorageAccountName = containerClient.AccountName;

            return containerDTO;
        }
    }
}
