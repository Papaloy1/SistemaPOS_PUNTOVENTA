namespace SistemaPOS.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public ApiResponse(bool success, string message, T data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public ApiResponse(bool success, string message, List<string> errors)
        {
            Success = success;
            Message = message;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operación exitosa")
            => new(true, message, data);

        public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
            => new(false, message, errors ?? new List<string> { message });
    }
}
