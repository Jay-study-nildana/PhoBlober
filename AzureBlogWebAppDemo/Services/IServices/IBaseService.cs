using AzureBlogWebAppDemo.DTO;

namespace AzureBlogWebAppDemo.Services.IServices
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}
