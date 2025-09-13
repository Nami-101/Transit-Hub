using System.Linq.Expressions;

namespace TransitHub.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Basic CRUD Operations
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        
        // Add/Update/Delete
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        
        // Paging and Sorting
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, 
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        
        // Stored Procedure Support
        Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string procedureName, params object[] parameters) where TResult : class, new();
        Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, params object[] parameters);
        Task<TResult?> ExecuteStoredProcedureScalarAsync<TResult>(string procedureName, params object[] parameters);
    }
}