using ApprovalSystem.Models;
using System.Linq.Expressions;

namespace ApprovalSystem.Data
{
    public class ApprovableRepository<K, T> : Repository<K, T>, IApprovableRepository<K, T> 
        where T : ApprovalBaseModel<K>, new()
    {
        public ApprovableRepository(ApplicationDbContext context, ILogger<IApprovableRepository<K, T>> logger)
            :base(context, logger)
        {
            
        }

        public override IQueryable<T> AsQueryable()
        {
            return DbSet.Where(t => t.ResourceState == ResourceState.Active);
        }

        public override IQueryable<T> AsQueryable(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression).Where(t => t.ResourceState == ResourceState.Active);
        }
    }
}