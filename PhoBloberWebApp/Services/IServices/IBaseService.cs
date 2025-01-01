using PhoBloberWebApp.DTO;

namespace PhoBloberWebApp.Services.IServices
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}
