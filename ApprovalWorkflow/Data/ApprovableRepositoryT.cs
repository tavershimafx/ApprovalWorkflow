using ApprovalSystem.Extensions;
using ApprovalSystem.Models;
using System.Linq.Expressions;

namespace ApprovalSystem.Data
{
    public class ApprovableRepository<K, T> : Repository<K, T>, IApprovableRepository<K, T> 
        where T : class, IApprovableEntity<K>, new()
    {
        public ApprovableRepository(ApplicationDbContext context, Serilog.ILogger logger)
            :base(context, logger)
        {
            
        }

        public override IQueryable<T> AsQueryable()
        {
            return DbSet.Where(t => t.ApprovalStatus == ApprovalStatus.Active && t.Status == EntityStatus.Active);
        }

        public override IQueryable<T> AsQueryable(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression.AndAlso(t => t.ApprovalStatus == ApprovalStatus.Active && t.Status == EntityStatus.Active));
        }
    }
}