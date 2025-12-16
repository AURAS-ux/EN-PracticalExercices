using System.Net;

namespace AMT.Domain.Utils;

public class Result<TValue, TException> where TValue : class where TException : Exception
{
    public bool IsSuccess { get; set; }
    public TValue? Value { get; set; }
    public string? ErrorMessage { get; set; }
    public TException? Exception { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    
    public Result(bool isSuccess, TValue? value = null, string? errorMessage = null, TException? exception = null, HttpStatusCode? statusCode = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        Exception = exception;
        StatusCode = statusCode;
    }

    public static Result<TValue, TException> Success(TValue value)
    {
        return new Result<TValue, TException>(true, value);
    }

    public static Result<TValue, TException> Failure(string errorMessage)
    {
        return new Result<TValue, TException>(false, null, errorMessage);
    }

    public static Result<TValue, TException> FailureWithException(string errorMessage, TException exception)
    {
        return new Result<TValue, TException>(false, null, errorMessage, exception);
    }

    public static Result<TValue, TException> Failure(string errorMessage, TException exception, HttpStatusCode statusCode)
    {
        return new Result<TValue, TException>(false, null, errorMessage, exception, statusCode);
    }
}
