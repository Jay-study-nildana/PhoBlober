using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PhoBloberWebAPI.DTO;
using PhoBloberWebAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using PhoBloberWebAPI.Services;
using PhoBloberWebAPI.Utilities;

namespace PhoBloberWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private const string Prefix = "";
        protected ResponseDto _response;
        private readonly IBlobStorageStuff _blobStorageStuff;
        private readonly StorageSettingsService _storageSettingsService;
        private readonly ControllerUtilities controlUtil;
        private readonly BlobUtilities blobutil;

        public BlobController(
            IBlobStorageStuff blobStorageStuff,
            StorageSettingsService storageSettingsService
            )
        {
            this._response = new ResponseDto();
            this._blobStorageStuff = blobStorageStuff;
            this._storageSettingsService = storageSettingsService;
            controlUtil = new ControllerUtilities();
            blobutil = new BlobUtilities();
        }

        [HttpPost("CreateNewContainer")]
        public async Task<IActionResult> CreateNewContainer(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                return StatusCode(400, controlUtil.CreateErrorResponse("Container name cannot be null or empty."));
            }

            try
            {
                // create the container and get the DTO
                var containerDTO = await blobutil.CreateBlobContainerAsync(containerName,_blobStorageStuff,_storageSettingsService);

                return StatusCode(200, controlUtil.CreateSuccessResponse(containerDTO, $"A container named '{containerName}' has been created."));
            }
            catch (Azure.RequestFailedException ex)
            {
                return StatusCode(400, (ex.ErrorCode == "InvalidResourceName"
                    ? controlUtil.CreateErrorResponse(ex.Message, new ContainerNameRulesDTO())
                    : controlUtil.CreateErrorResponse(ex.Message)));
            }
            catch (Exception ex)
            {

                if(controlUtil.ContainsRetryFailed(ex.Message))
                {
                    return StatusCode(503, controlUtil.CreateErrorResponse(ex.Message));
                }

                return StatusCode(500, controlUtil.CreateErrorResponse(ex.Message));
            }
        }

        //get all containers
        [HttpGet]
        [Route("GetAllContainers")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys(_storageSettingsService);
                string? storageConnectionString = storageaccesskeys;

                // Create a client that can authenticate with a connection string
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

                GetAllContainersDTO getAllContainersDTO = blobutil.GetAllContainers(blobServiceClient);
                if (getAllContainersDTO.ContainerIds.Count == 0)
                {
                    _response.Message = "Looks like there are no containers in the current account.";
                    return StatusCode(404, _response);
                }
                else
                {
                    _response.Result = getAllContainersDTO;
                    _response.Message = "Total of " + getAllContainersDTO.ContainerCount + " Containers Loaded";
                    return StatusCode(200, _response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, controlUtil.CreateErrorResponse(ex.Message));
            }            
        }

		[HttpPost("SetContainerPublic")]
		public async Task<IActionResult> SetContainerPublic(string containerName)
		{
			try
			{

                 var containerDTO = blobutil.ToggleContainerPublic(containerName,_blobStorageStuff,_storageSettingsService);

				_response.Result = containerDTO;
				_response.Message = "container" + containerName + " anonymous public access is turned on. ";

                return StatusCode(200, _response);
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
                return StatusCode(404, _response);
            }
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message += ex.Message;
                return StatusCode(500, controlUtil.CreateErrorResponse(ex.Message));
            }
		}

		[HttpPost("UploadPhoto")]
        public async Task<IActionResult> UploadPhoto(PhotoUploadDTO photoUpload)
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys(_storageSettingsService);

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
					//return _response;
                    return StatusCode(401, _response);
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
                return StatusCode(200, _response);

            }
            catch (Azure.RequestFailedException ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
                return StatusCode(400, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
                return StatusCode(500, controlUtil.CreateErrorResponse(ex.Message));
            }
        }

        //get all blobs aka images
        [HttpGet]
        [Route("GetAllBlobs")]
        public async Task<IActionResult> GetGetAllBlobs(string containerName)
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys(_storageSettingsService);
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
                    return StatusCode(404, _response);
                }
                else
                {
                    _response.Result = getAllBlobsDTO;
                    _response.Message = "Total of " + getAllBlobsDTO.blobCount + " Images Loaded";
                    return StatusCode(200, _response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, controlUtil.CreateErrorResponse(ex.Message));
            }
        }


    }
}
