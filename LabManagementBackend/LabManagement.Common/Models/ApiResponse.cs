namespace LabManagement.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public ApiResponse()
        {
        }

        public ApiResponse(T data, string message = "Success")
        {
            Success = true;
            Message = message;
            Data = data;
        }

        public ApiResponse(string message, List<string> errors)
        {
            Success = false;
            Message = message;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>(data, message);
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>(message, errors ?? new List<string> { message });
        }
    }
}
