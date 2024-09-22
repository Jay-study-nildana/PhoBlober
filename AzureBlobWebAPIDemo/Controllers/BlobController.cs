using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobWebAPIDemo.BlobHelpers;
using AzureBlobWebAPIDemo.DTO;
using AzureBlobWebAPIDemo.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace AzureBlobWebAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private const string Prefix = "";
        protected ResponseDto _response;
        private readonly IBlobStorageStuff _blobStorageStuff;

        public BlobController(
            IBlobStorageStuff blobStorageStuff
            )
        {
            this._response = new ResponseDto();
            this._blobStorageStuff = blobStorageStuff;
        }

        [HttpPost("CreateNewContainer")]
        public async Task<ResponseDto> CreateNewContainer(string containerName)
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys();

                string? storageConnectionString = storageaccesskeys;

                // Create a client that can authenticate with a connection string
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

                //Create a unique name for the container
                containerName += Guid.NewGuid().ToString();

                // Create the container and return a container client object
                BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

                var containerDTO = new ContainerCreatedDTO();
                containerDTO.ContainerName = containerName;
                containerDTO.DateTimeOfCreation = DateTime.UtcNow.ToString();
                containerDTO.ContainerPrimaryUri = containerClient.Uri;
                containerDTO.StorageAccountName = containerClient.AccountName;

                _response.Result = containerDTO;
                _response.Message = "A container named '" + containerName + "' has been created. ";
            }
            catch (Azure.RequestFailedException ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;

                if(ex.ErrorCode== "InvalidResourceName")
                {
                    var ContainerNameRulesDTO = new ContainerNameRulesDTO();
                    _response.Result = ContainerNameRulesDTO;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
            }
            return _response;
        }

        //get all containers
        [HttpGet]
        [Route("GetAllContainers")]
        public async Task<ResponseDto> Get()
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys();
                string? storageConnectionString = storageaccesskeys;

                // Create a client that can authenticate with a connection string
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
                // Get the blob helper, to get all containers
                BlobHelperContainer blobHelperContainer = new BlobHelperContainer();

                GetAllContainersDTO getAllContainersDTO = blobHelperContainer.GetAllContainers(blobServiceClient);
                if (getAllContainersDTO.ContainerIds.Count == 0)
                {
                    _response.Message = "Looks like there are no containers in the current account.";
                }
                else
                {
                    _response.Result = getAllContainersDTO;
                    _response.Message = "Total of " + getAllContainersDTO.ContainerCount + " Containers Loaded";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

		[HttpPost("SetContainerPublic")]
		public async Task<ResponseDto> SetContainerPublic(string containerName)
		{
			try
			{
				var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys();

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

				_response.Result = containerDTO;
				_response.Message = "container" + containerName + " anonymous public access is turned on. ";
			}
			catch (Azure.RequestFailedException ex)
			{
				_response.IsSuccess = false;
				_response.Message += ex.Message;

				if (ex.ErrorCode == "InvalidResourceName")
				{
					var ContainerNameRulesDTO = new ContainerNameRulesDTO();
					_response.Result = ContainerNameRulesDTO;
				}
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message += ex.Message;
			}
			return _response;
		}

		[HttpPost("UploadPhoto")]
        public async Task<ResponseDto> UploadPhoto(PhotoUploadDTO photoUpload)
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys();

                string? storageConnectionString = storageaccesskeys;

                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

                //apply the default container name if no container name is provided
                if(photoUpload.containerName == null)
                {
                    photoUpload.containerName = _blobStorageStuff.GiveMeDefaultContainerName();
                }

                BlobContainerClient containerClient =  blobServiceClient.GetBlobContainerClient(photoUpload.containerName);

                var publicaccesssituation = (BlobContainerAccessPolicy)containerClient.GetAccessPolicy();

				//Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer
                if(publicaccesssituation.BlobPublicAccess != PublicAccessType.BlobContainer)
                {
					_response.IsSuccess = false;
					_response.Message += "Container has no public access. Unable to Upload Image";
					return _response;
				}

				//rename the file.
				//1. I should not use the name as provided by the user
				//2. if I don't make any name changes, I will get an error because, 
				//duplicate files are obviously not allowed. 
				var uploadFileName = Guid.NewGuid().ToString();
                //this is the name that will be used for the image creation.
                var getExtensionOfUploadedImage = System.IO.Path.GetExtension(photoUpload.Image?.FileName);
				//uploadFileName += photoUpload.Image?.FileName;
				uploadFileName =uploadFileName+getExtensionOfUploadedImage;
				BlobClient blobClient = containerClient.GetBlobClient(uploadFileName);

                //this also works.
                //copying to stream and then pushing the stream to the cloud

                //using (var stream = System.IO.File.Create(uploadFileName))
                //{
                //    await photoUpload.Image.CopyToAsync(stream);
                //    stream.Position = 0;
                //    var info2 = await blobClient.UploadAsync(stream);
                //}

                //this also works. preferred, simply way. directly using Azure SDK.
                var info = await blobClient.UploadAsync(photoUpload.Image?.OpenReadStream());

                PhotoUploadedDTO photoUploadedDTO = new PhotoUploadedDTO();
                photoUploadedDTO.PhotoName = photoUpload.PhotoName;
                photoUploadedDTO.AccountName = blobClient.AccountName;
                photoUploadedDTO.BlobName = blobClient.Name;
                photoUploadedDTO.BlobContainerName = blobClient.BlobContainerName;
                photoUploadedDTO.BlobUri = blobClient.Uri;
                photoUploadedDTO.PhotoDescription = photoUpload.PhotoDescription;

                _response.Result = photoUploadedDTO;
                _response.Message = "Photo Uploaded Successfully";

            }
            catch (Azure.RequestFailedException ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
            }
            return _response;
        }

        //get all blobs aka images
        [HttpGet]
        [Route("GetAllBlobs")]
        public async Task<ResponseDto> GetGetAllBlobs(string containerName)
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys();
                string? storageConnectionString = storageaccesskeys;

                // Create a client that can authenticate with a connection string
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

                //public BlobContainerClient (string connectionString, string blobContainerName);
                var blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);

                var containerURI = blobContainerClient.Uri;
                GetAllBlobsDTO getAllBlobsDTO = new GetAllBlobsDTO();

                int? segmentSize = null;
                // Call the listing operation and return pages of the specified size.
                var resultSegment = blobContainerClient.GetBlobsAsync()
                    .AsPages(default, segmentSize);

                // Enumerate the blobs returned for each page.
                await foreach (Page<BlobItem> blobPage in resultSegment)
                {
                    foreach (BlobItem blobItem in blobPage.Values)
                    {
                        var x = blobItem;
                        var bloburi = x.Name;
                        var fullbloburi = Path.Combine([containerURI.AbsoluteUri,blobItem.Name]);
                        //is there a better way to build this URL instead of doing basic string concatenation
                        var fullbloburi2 = containerURI.AbsoluteUri+"/"+blobItem.Name;

                        getAllBlobsDTO.BlobFullURL.Add(fullbloburi);
                        getAllBlobsDTO.blobCount = getAllBlobsDTO.blobCount + 1;
                    }
                }

                if (getAllBlobsDTO.BlobFullURL.Count == 0)
                {
                    _response.Message = "Looks like there are no containers in the current account.";
                }
                else
                {
                    _response.Result = getAllBlobsDTO;
                    _response.Message = "Total of " + getAllBlobsDTO.blobCount + " Images Loaded";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


    }
}
