using System.Net;

namespace AMT.Domain.Utils;

public class Result<TValue, TException> where TValue : class where TException : Exception
{
    public bool IsSuccess { get; set; }
    public TValue? Value { get; set; }
    public List<string>? ErrorMessages { get; set; }
    public List<TException>? Exceptions { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    
    public Result(bool isSuccess, TValue? value = null, List<string>? errorMessage = null, List<TException>? exception = null, HttpStatusCode? statusCode = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessages = errorMessage;
        Exceptions = exception;
        StatusCode = statusCode;
    }

    public static Result<TValue, TException> Success(TValue value)
    {
        return new Result<TValue, TException>(true, value);
    }

    public static Result<TValue, TException> Failure(List<string> errorMessage)
    {
        return new Result<TValue, TException>(false, null, errorMessage);
    }

    public static Result<TValue, TException> FailureWithException(List<string> errorMessage, List<TException> exception)
    {
        return new Result<TValue, TException>(false, null, errorMessage, exception);
    }

    public static Result<TValue, TException> Failure(List<string> errorMessage, List<TException> exception, HttpStatusCode statusCode)
    {
        return new Result<TValue, TException>(false, null, errorMessage, exception, statusCode);
    }
}
