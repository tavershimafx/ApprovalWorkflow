using ApprovalSystem.Extensions;
using ApprovalSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Linq.Expressions;

namespace ApprovalSystem.Data
{
    public class Repository<K, T> : IRepository<K, T> where T : class, IModelBase<K>, new()
    {
        private readonly Serilog.ILogger _logger;
        public Repository(ApplicationDbContext context, Serilog.ILogger logger)
        {
            Context = context;
            DbSet = Context.Set<T>();
            _logger = logger;
        }

        protected ApplicationDbContext Context { get; }

        protected DbSet<T> DbSet { get; }

        public async Task InsertAsync(T model)
        {
            await DbSet.AddAsync(model);
        }

        public void Insert(T model)
        {
            DbSet.Add(model);
        }

        public async Task InsertOrUpdate(T model)
        {
            var entity = Context.Entry(model);
            if (entity != null)
            {
                Update(model);
            }
            else
            {
                await InsertAsync(model);
            }
        }

        public async Task InsertRangeAsync(IEnumerable<T> model)
        {
            await DbSet.AddRangeAsync(model);
        }

        public void InsertRange(IEnumerable<T> model)
        {
            DbSet.AddRange(model);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Context.Database.CommitTransaction();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        public virtual IQueryable<T> AsQueryable()
        {
            ThrowIfApprovableEntity();
            return DbSet.Where(t => t.Status == EntityStatus.Active);
        }

        public virtual IQueryable<T> AsQueryable(Expression<Func<T, bool>> expression)
        {
            ThrowIfApprovableEntity();
            return DbSet.Where(expression.AndAlso(t => t.Status == EntityStatus.Active));
        }

        public virtual IQueryable<T> AsUnFilteredQueryable()
        {
            return DbSet.Where(t => t.Status != EntityStatus.PermanentDelete);
        }

        public virtual IQueryable<T> AsUnFilteredQueryable(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression.AndAlso(t => t.Status != EntityStatus.PermanentDelete));
        }

        public virtual T FirstOrDefault()
        {
            ThrowIfApprovableEntity();
            return DbSet.FirstOrDefault(t => t.Status != EntityStatus.PermanentDelete);
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            ThrowIfApprovableEntity();
            return DbSet.FirstOrDefault(expression.AndAlso(t => t.Status != EntityStatus.PermanentDelete));
        }

        public async Task DeleteAsync(K key)
        {
            var entity = await DbSet.FindAsync(key);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        public Task DeleteAsync(Func<T, bool> expression)
        {
            var entities = DbSet.Where(expression);
            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }
            return Task.CompletedTask;
        }

        public void DeleteRange(IEnumerable<T> models)
        {
            DbSet.RemoveRange(models);
        }

        public void Delete(T model)
        {
            if (model != null)
                DbSet.Remove(model);
        }

        public void Update(T model)
        {
            if (model != null)
            {
                Context.ShadowIds.Add(model.Id);
                DbSet.Update(model);
            }
        }

        private void ThrowIfApprovableEntity()
        {
            if (typeof(T).IsAssignableTo(typeof(IApprovableEntity<K>)))
                throw new InvalidOperationException("Cannot perform the requested operation because the related entity is of type" +
                    "IApprovableEntity. Use the appropriate repository interface to access or modify the related entity");
        }
    }
}