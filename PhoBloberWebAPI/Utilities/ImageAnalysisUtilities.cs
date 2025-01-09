using Azure.Storage.Blobs;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using PhoBloberWebAPI.DTO;
using PhoBloberWebAPI.Services;
using PhoBloberWebAPI.Services.IServices;

namespace PhoBloberWebAPI.Utilities
{
    public class ImageAnalysisUtilities
    {
        public async Task<PhotoAnalysedDTO> AnalysePhoto(PhotoUploadDTO photoUpload, BlobContainerClient containerClient, IComputerVisionStuff _computerVisionStuff, CVSettingsService _cVSettingsService)
        {
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
            var CVKeys = _computerVisionStuff.GetMeComputerVisionSettings(_cVSettingsService);

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

            return analysed;
        }

        public async Task<PhotoOCRedDTO> OCRPhoto(PhotoUploadDTO photoUpload, BlobContainerClient containerClient, IComputerVisionStuff _computerVisionStuff, CVSettingsService _cVSettingsService)
        {
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
            string READ_TEXT_URL_IMAGE = blobClient.Uri.ToString();
            //get the keys
            var CVKeys = _computerVisionStuff.GetMeComputerVisionSettings(_cVSettingsService);

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

            return analysed;
        }

        public bool ContainsLargeImage(string inputString)
        {
            return inputString.Contains("should not be larger than");
        }
    }
}
