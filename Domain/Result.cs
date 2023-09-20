namespace Domain
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode {get;set;}
        public static Result Succeed() => new() { Success = true, StatusCode = 200 };
        public static Result Fail(string message, int statusCode = 500) => new() { Success = false, Message = message, StatusCode = statusCode };

    }


    public class Result<T>
    {
        public bool Success { get; set; }
        public T Value { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public static Result<T> Succeed(T value, int statusCode = 200)
        {
            return new Result<T> { Success = true, Value = value, StatusCode = statusCode };
        }

        public static Result<T> Fail(string message, int statusCode = 500) 
        { 
            return new Result<T> { Success = false, Message = message, StatusCode = statusCode }; 
        }

    }
}
