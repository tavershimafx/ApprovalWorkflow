using ApprovalSystem.Models;

namespace ApprovalSystem.Data
{
    public interface IApprovableRepository<K, T>: IRepository<K, T>  where T: IApprovableEntity<K>, new()
    {
        
    }
}
