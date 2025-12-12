namespace AMT.Domain.Utils;

public class Result<TValue, TError, TException> where TValue : class where TError : class where TException : Exception
{
    public TValue? Value { get; set; }
    public TError? ErrorMessage { get; set; }
    public TException? Exception { get; set; }
    public Result(TValue? value, TError? errorMessage, TException? exception)
    {
        Value = value;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public static Result<TValue, TError, TException> Success(TValue value)
    {
        return new Result<TValue, TError, TException>(value, null, null);
    }

    public static Result<TValue, TError, TException> Failure(TError errorMessage)
    {
        return new Result<TValue, TError, TException>(null, errorMessage, null);
    }

    public static Result<TValue, TError, TException> FailureWithException(TError error, TException exception)
    {
        return new Result<TValue, TError, TException>(null, error, exception);
    }
}
