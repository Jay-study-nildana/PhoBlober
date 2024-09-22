using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using AzureBlobWebAPIDemo.DTO;
using AzureBlobWebAPIDemo.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Net;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AzureBlobWebAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageAnalysisController : ControllerBase
    {
		private readonly IComputerVisionStuff _computerVisionStuff;
		private readonly IBlobStorageStuff _blobStorageStuff;
		protected ResponseDto _response;
		public ImageAnalysisController(
            IComputerVisionStuff computerVisionStuff,
			IBlobStorageStuff blobStorageStuff
			) 
        { 
            _computerVisionStuff = computerVisionStuff;
			_blobStorageStuff = blobStorageStuff;
			this._response = new ResponseDto();
		}

		[HttpPost("UploadForPhotoAnalysis")]
		public async Task<ResponseDto> UploadForPhotoAnalysis(PhotoUploadDTO photoUpload)
		{
			try
			{
				var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys();

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
				uploadFileName = uploadFileName + getExtensionOfUploadedImage;
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

				//okay, image analysis
				//const string ANALYZE_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png";
				string ANALYZE_URL_IMAGE = blobClient.Uri.ToString();
				//get the keys
				var CVKeys = _computerVisionStuff.GetMeComputerVisionSettings();

				// Create a client
				ComputerVisionClient client =
				  new ComputerVisionClient(new ApiKeyServiceClientCredentials(CVKeys.VISION_KEY))
				  { Endpoint = CVKeys.VISION_ENDPOINT };

				List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
				{
					VisualFeatureTypes.Tags
				};
				// Analyze the URL image 
				ImageAnalysis results = await client.AnalyzeImageAsync(ANALYZE_URL_IMAGE, visualFeatures: features);

				List<ImageTag> imageTags = new List<ImageTag>();

				// Image tags and their confidence score
				foreach (var tag in results.Tags)
				{
					imageTags.Add(tag);
				}

				var analysed = new PhotoAnalysedDTO();
				analysed.imageTags = imageTags;
				analysed.PhotoName = photoUpload.PhotoName;
				analysed.AccountName = blobClient.AccountName;
				analysed.BlobName = blobClient.Name;
				analysed.BlobContainerName = blobClient.BlobContainerName;
				analysed.BlobUri = blobClient.Uri;
				analysed.PhotoDescription = photoUpload.PhotoDescription;

				_response.Result = analysed;
				_response.Message = "Photo Analysed Successfully";

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

        [HttpPost("UploadForOCRAnalysis")]
        public async Task<ResponseDto> UploadForOCRAnalysis(PhotoUploadDTO photoUpload)
        {
            try
            {
                var storageaccesskeys = _blobStorageStuff.GiveMeAccessKeys();

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
                uploadFileName = uploadFileName + getExtensionOfUploadedImage;
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

                //okay, ocr analysis
                //string READ_TEXT_URL_IMAGE = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/ComputerVision/Images/printed_text.jpg";
                string READ_TEXT_URL_IMAGE = blobClient.Uri.ToString();
                //get the keys
                var CVKeys = _computerVisionStuff.GetMeComputerVisionSettings();

                // Create a client
                ComputerVisionClient client =
                  new ComputerVisionClient(new ApiKeyServiceClientCredentials(CVKeys.VISION_KEY))
                  { Endpoint = CVKeys.VISION_ENDPOINT };

                string urlFile = READ_TEXT_URL_IMAGE;

                // Read text from URL
                var textHeaders = await client.ReadAsync(urlFile);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders.OperationLocation;
                Thread.Sleep(2000);

                // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
                // We only need the ID and not the full URL
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // Extract the text
                ReadOperationResult results;
                do
                {
                    results = await client.GetReadResultAsync(Guid.Parse(operationId));
                }
                while ((results.Status == OperationStatusCodes.Running ||
                    results.Status == OperationStatusCodes.NotStarted));

                // Collect the found text.
                var foundlines = new List<string>();
                var textUrlFileResults = results.AnalyzeResult.ReadResults;
                foreach (ReadResult page in textUrlFileResults)
                {
                    foreach (Line line in page.Lines)
                    {
                        //Console.WriteLine(line.Text);
                        foundlines.Add(line.Text);
                    }
                }

                var analysed = new PhotoOCRedDTO();
                analysed.foundlines = foundlines;
                analysed.PhotoName = photoUpload.PhotoName;
                analysed.AccountName = blobClient.AccountName;
                analysed.BlobName = blobClient.Name;
                analysed.BlobContainerName = blobClient.BlobContainerName;
                analysed.BlobUri = blobClient.Uri;
                analysed.PhotoDescription = photoUpload.PhotoDescription;

                _response.Result = analysed;
                _response.Message = "Photo Analysed Successfully";

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


    }
}
