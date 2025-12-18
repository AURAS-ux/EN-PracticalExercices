using System;

namespace AMT.Infrastructure.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string jsonValidationResults) : base(jsonValidationResults)
    {
        
    }
}
