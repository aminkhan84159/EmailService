namespace MeshComm.Core.Interfaces
{
    public interface IGenericService<T> : IDisposable where T : class
    {
        Task<bool> AddAsync(T entity);
        Task<bool> DeleteAsync(int entityId);
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(Guid entityId);
        Task<T?> GetByIdAsync(int entityId);
        Task<bool> UpdateAsync(T entity);
    }
}
