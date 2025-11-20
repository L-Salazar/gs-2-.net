namespace RemoteReady.Models
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; } = 200;
        public T? Value { get; set; }

        private OperationResult() { }

        public static OperationResult<T> Success(T value, int statusCode = 200)
        {
            return new OperationResult<T> { IsSuccess = true, Value = value, StatusCode = statusCode };
        }

        public static OperationResult<T> Failure(string error, int statusCode = 400)
        {
            return new OperationResult<T> { IsSuccess = false, Error = error, StatusCode = statusCode };
        }
    }
}

