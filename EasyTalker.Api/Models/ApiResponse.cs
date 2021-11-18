namespace EasyTalker.Api.Models
{
    public class ApiResponse<T>
    {
        public static ApiResponse<T> Success(T data) => new(data);
        public static ApiResponse<T> Failure(string error) => new(error);

        public T Data { get; }
        public string Error { get; }

        public bool IsError => !string.IsNullOrEmpty(Error);
        public bool IsSuccess => string.IsNullOrEmpty(Error);

        private ApiResponse(T data)
        {
            Data = data;
        }

        private ApiResponse(string error)
        {
            Error = error;
        }
    }
}