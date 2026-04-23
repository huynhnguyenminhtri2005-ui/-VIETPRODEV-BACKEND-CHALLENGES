namespace TestAPIChallenges.Responses
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } // "success" hoặc "error"
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Violations { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Thành công") =>
            new ApiResponse<T> { Status = "success", Message = message, Data = data };

        public static ApiResponse<T> Error(string message, List<string> violations = null) =>
            new ApiResponse<T> { Status = "error", Message = message, Violations = violations };
    }
}