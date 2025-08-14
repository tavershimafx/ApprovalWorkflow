using ApprovalSystem.Dtos;
using ApprovalSystem.Types;

namespace ApprovalSystem.Services
{
    public interface IRoleService : IApplicationScopedService
    {
        /// <summary>
        /// Gets the list of roles in the system.
        /// </summary>
        /// <returns></returns>
        TaskResult<IEnumerable<RoleListDto>> GetRoles();

        TaskResult<IEnumerable<RoleListDto>> GetRolesUnfiltered();

        /// <summary>
        /// Adds a new role to the system. This is a soft add, meaning the
        ///  role is not immediately effective until its approved
        /// </summary>
        /// <returns></returns>
        TaskResult AddRole(AddRoleDto model);

        /// <summary>
        /// Updates an existing role in the system. This is a soft update, meaning the 
        /// role is not immediately updated to the database until its approved
        /// </summary>
        /// <returns></returns>
        TaskResult UpdateRole(AddRoleDto model);

        /// <summary>
        /// Deletes a role from the system. This is a soft delete, meaning the 
        ///  role is not immediately deleted from the database until its approved
        /// </summary>
        /// <returns></returns>
        TaskResult DeleteRole(long id);
    }
}
