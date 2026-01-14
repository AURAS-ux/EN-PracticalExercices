using System;

namespace AMT.Infrastructure.Exceptions;

public class GenericNotFound<T,TIdentifier>(TIdentifier id) : Exception($"{typeof(T).Name} with Id {id} was not found.") where T : class
{
}
