using Azure.Storage.Blobs;
using PhoBloberWebAPI.DTO;

namespace PhoBloberWebAPI.BlobHelpers
{
    public class BlobHelperContainer
    {
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
    }
}
