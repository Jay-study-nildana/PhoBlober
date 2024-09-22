using AzureBlogWebAppDemo.DTO;
using AzureBlogWebAppDemo.Services.IServices;
using AzureBlogWebAppDemo.Utility;

namespace AzureBlogWebAppDemo.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IBaseService _baseService;

        public PhotoService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetPhotosAsync(string containerName)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.PhotoAPIBase + "/api/Blob/GetAllBlobs?containerName=" + containerName
            });
        }

        public async Task<ResponseDto?> UploadPhotoAsync(PhotoUploadDTO photoUploadDTO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = photoUploadDTO,
                Url = SD.PhotoAPIBase + "/api/Blob/UploadPhoto",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

		public async Task<ResponseDto?> UploadForPhotoAnalysis(PhotoUploadDTO photoUploadDTO)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = photoUploadDTO,
				Url = SD.PhotoAPIBase + "/api/ImageAnalysis/UploadForPhotoAnalysis",
				ContentType = SD.ContentType.MultipartFormData
			});
		}

        public async Task<ResponseDto?> UploadForOCRAnalysis(PhotoUploadDTO photoUploadDTO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = photoUploadDTO,
                Url = SD.PhotoAPIBase + "/api/ImageAnalysis/UploadForOCRAnalysis",
                ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}
