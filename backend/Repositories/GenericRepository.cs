using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using TransitHub.Data;
using TransitHub.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;

namespace TransitHub.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TransitHubDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(TransitHubDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, 
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }
            return await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string procedureName, params object[] parameters) where TResult : class, new()
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            
            // Add parameters (they should be SqlParameter objects)
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is SqlParameter sqlParam)
                    {
                        command.Parameters.Add(sqlParam);
                    }
                }
            }

            await _context.Database.OpenConnectionAsync();
            
            try
            {
                using var reader = await command.ExecuteReaderAsync();
                var results = new List<TResult>();
                
                while (await reader.ReadAsync())
                {
                    var item = new TResult();
                    var properties = typeof(TResult).GetProperties();
                    
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var columnName = reader.GetName(i);
                        var property = properties.FirstOrDefault(p => 
                            string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
                        
                        if (property != null && reader[i] != DBNull.Value)
                        {
                            var value = Convert.ChangeType(reader[i], property.PropertyType);
                            property.SetValue(item, value);
                        }
                    }
                    
                    results.Add(item);
                }
                
                return results;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public virtual async Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, params object[] parameters)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            
            // Add parameters (they should be SqlParameter objects)
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is SqlParameter sqlParam)
                    {
                        command.Parameters.Add(sqlParam);
                    }
                }
            }

            await _context.Database.OpenConnectionAsync();
            
            try
            {
                return await command.ExecuteNonQueryAsync();
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public virtual async Task<TResult?> ExecuteStoredProcedureScalarAsync<TResult>(string procedureName, params object[] parameters)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            
            // Add parameters (they should be SqlParameter objects)
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is SqlParameter sqlParam)
                    {
                        command.Parameters.Add(sqlParam);
                    }
                }
            }

            await _context.Database.OpenConnectionAsync();
            
            try
            {
                var result = await command.ExecuteScalarAsync();
                return result == null || result == DBNull.Value ? default(TResult) : (TResult)result;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }
}