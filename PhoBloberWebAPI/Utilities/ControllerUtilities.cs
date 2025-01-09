using PhoBloberWebAPI.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhoBloberWebAPI.Utilities
{
    public class ControllerUtilities
    {
        public ResponseDto CreateErrorResponse(string message, object? result = null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = message,
                Result = result
            };
        }

        public ResponseDto CreateSuccessResponse(object result, string message)
        {
            return new ResponseDto
            {
                IsSuccess = true,
                Message = message,
                Result = result
            };
        }

        public bool ContainsRetryFailed(string inputString)
        {
            return inputString.Contains("Retry failed");
        }
    }
}
