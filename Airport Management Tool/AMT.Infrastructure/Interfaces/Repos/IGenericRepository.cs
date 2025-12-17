using System;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T entity);
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Update(T entity);
    void Delete(int id);
}
