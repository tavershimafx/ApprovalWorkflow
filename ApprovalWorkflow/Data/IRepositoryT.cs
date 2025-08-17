using ApprovalSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace ApprovalSystem.Data
{
    /// <summary>
    /// Generic repository interface for CRUD operations on a model type.
    /// </summary>
    /// <typeparam name="K">The key type</typeparam>
    /// <typeparam name="T">The model class</typeparam>
    public interface IRepository<K, T> where T : IModelBase<K>, new()
    {
        IQueryable<T> AsQueryable();

        IQueryable<T> AsQueryable(Expression<Func<T, bool>> expression);

        IQueryable<T> AsUnFilteredQueryable();

        IQueryable<T> AsUnFilteredQueryable(Expression<Func<T, bool>> expression);

        T FirstOrDefault();

        T FirstOrDefault(Expression<Func<T, bool>> expression);

        IDbContextTransaction BeginTransaction();

        void CommitTransaction();

        /// <summary>
        /// Saves all database changes made to the backing store
        /// </summary>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        /// <returns></returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Saves all database changes made to the backing store
        /// </summary>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        /// <returns></returns>
        void SaveChanges();

        void Insert(T model);

        Task InsertAsync(T model);

        Task InsertOrUpdate(T model);

        void InsertRange(IEnumerable<T> model);

        Task InsertRangeAsync(IEnumerable<T> model);

        void Delete(T entity);

        Task DeleteAsync(K key);

        Task DeleteAsync(Func<T, bool> expression);

        void DeleteRange(IEnumerable<T> entities);

        void Update(T model);
    }
}
