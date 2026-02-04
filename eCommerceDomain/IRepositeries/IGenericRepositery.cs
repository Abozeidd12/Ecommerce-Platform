using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceDomain.IRepositeries
{
    public interface IGenericRepository<T> where T : class
    {
        // Read operations
        Task<T?> GetByIdAsync(params object[] keyValues);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        // Read with includes
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindIncludingAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties);

        // *** ADD THIS NEW METHOD ***
        Task<IEnumerable<T>> GetWithAdvancedIncludesAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Func<IQueryable<T>, IQueryable<T>>[] includes);

        // Write operations
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        // Pagination
        Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        //Raw sql
        Task<IEnumerable<T>> ExecuteRawSql<T>(string sqlQuery, params SqlParameter[] parameters) where T : class;
    }
}
