using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using PhoBloberWebAPI.DTO;
using PhoBloberWebAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Net;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using PhoBloberWebAPI.Services;
using PhoBloberWebAPI.Utilities;

namespace PhoBloberWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageAnalysisController : ControllerBase
    {
		private readonly IComputerVisionStuff _computerVisionStuff;
		private readonly IBlobStorageStuff _blobStorageStuff;
        private readonly CVSettingsService _cVSettingsService;
        private readonly StorageSettingsService _storageSettingsService;
        protected ResponseDto _response;
        private readonly ImageAnalysisUtilities imageutil;
        private readonly ControllerUtilities controlUtil;
        public ImageAnalysisController(
            IComputerVisionStuff computerVisionStuff,
			IBlobStorageStuff blobStorageStuff,
            CVSettingsService cVSettingsService,
            StorageSettingsService storageSettingsService
            ) 
        { 
            _computerVisionStuff = computerVisionStuff;
			_blobStorageStuff = blobStorageStuff;
            this._storageSettingsService = storageSettingsService;
            this._cVSettingsService = cVSettingsService;
			this._response = new ResponseDto();
            imageutil = new ImageAnalysisUtilities();
            controlUtil = new ControllerUtilities();

        }

		[HttpPost("UploadForPhotoAnalysis")]
		public async Task<IActionResult> UploadForPhotoAnalysis(PhotoUploadDTO photoUpload)
		{
			try
			{
				var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys(_storageSettingsService);

				string? storageConnectionString = storageaccesskeys;

				BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

				//apply the default container name if no container name is provided
				if (photoUpload.containerName == null)
				{
					photoUpload.containerName = _blobStorageStuff.GiveMeDefaultContainerName();
				}

				BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(photoUpload.containerName);

				var publicaccesssituation = (BlobContainerAccessPolicy)containerClient.GetAccessPolicy();

				//Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer
				if (publicaccesssituation.BlobPublicAccess != PublicAccessType.BlobContainer)
				{
					_response.IsSuccess = false;
					_response.Message += "Container has no public access. Unable to Upload Image";
					//return _response;
                    return StatusCode(401, _response);
                }

                var analysed = await imageutil.AnalysePhoto(photoUpload,containerClient,_computerVisionStuff,_cVSettingsService);


                _response.Result = analysed;
				_response.Message = "Photo Analysed Successfully";
                return StatusCode(200, _response);

            }
			catch (Azure.RequestFailedException ex)
			{
				_response.IsSuccess = false;
				_response.Message += ex.Message;
                return StatusCode(503, _response);
            }
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message += ex.Message;
                return StatusCode(500, _response);
            }
			//return _response;
		}

        [HttpPost("UploadForOCRAnalysis")]
        public async Task<IActionResult> UploadForOCRAnalysis(PhotoUploadDTO photoUpload)
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys(_storageSettingsService);

                string? storageConnectionString = storageaccesskeys;

                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

                //apply the default container name if no container name is provided
                if (photoUpload.containerName == null)
                {
                    photoUpload.containerName = _blobStorageStuff.GiveMeDefaultContainerName();
                }

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(photoUpload.containerName);

                var publicaccesssituation = (BlobContainerAccessPolicy)containerClient.GetAccessPolicy();

                //Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer
                if (publicaccesssituation.BlobPublicAccess != PublicAccessType.BlobContainer)
                {
                    _response.IsSuccess = false;
                    _response.Message += "Container has no public access. Unable to Upload Image";
                    //return _response;
                    return StatusCode(401, _response);
                }

                //TODO. Check if file size is more than 4 MB before you do OCR analysis.

                var analysed = await imageutil.OCRPhoto(photoUpload,containerClient,_computerVisionStuff,_cVSettingsService);

                _response.Result = analysed;
                _response.Message = "Photo Analysed Successfully";
                return StatusCode(200, _response);

            }
            catch (Azure.RequestFailedException ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
                return StatusCode(503, _response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, controlUtil.CreateErrorResponse(ex.Message + "Also, you could check if the image size is more than 4MB. That's one common reason why OCR fails"));
            }
        }
    }
}
