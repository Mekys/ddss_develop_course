namespace Application.Repositories;

public interface IBaseRepository<T>
{
    Task<T> GetById(Guid id);
    Task<List<T>> GetAll();
    Task<T> Update(T entity);
    Task Delete(T entity);
    Task<T> Create(T entity);
}