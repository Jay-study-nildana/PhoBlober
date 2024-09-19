using AzureBlogWebAppDemo.DTO;

namespace AzureBlogWebAppDemo.Services.IServices
{
    public interface IPhotoService
    {
        Task<ResponseDto?> UploadPhotoAsync(PhotoUploadDTO photoUploadDTO);
        Task<ResponseDto?> GetPhotosAsync(string containerName);

    }
}
