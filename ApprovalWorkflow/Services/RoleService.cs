using ApprovalSystem.Data;
using ApprovalSystem.Dtos;
using ApprovalSystem.Models;
using ApprovalSystem.Types;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly IApprovableRepository<long, Role> _roleRepository;

        public RoleService(IApprovableRepository<long, Role> roleRepo)
        {
            _roleRepository = roleRepo;
        }

        public TaskResult AddRole(AddRoleDto model)
        {
            var rl = new Role()
            {
                Name = model.Name,
                Description = model.Description,
            };
            _roleRepository.Insert(rl);

            _roleRepository.SaveChanges();
            return TaskResult.Ok();
        }

        public TaskResult DeleteRole(long id)
        {
            throw new NotImplementedException();
        }

        public TaskResult<IEnumerable<RoleListDto>> GetRoles()
        {
            var roles = _roleRepository.AsQueryable().AsNoTracking()
                .Select(r => RoleListDto.FromRole(r)).AsEnumerable();

            return TaskResult<IEnumerable<RoleListDto>>.Ok(roles);
        }

        public TaskResult<IEnumerable<RoleListDto>> GetRolesUnfiltered()
        {
            var roles = _roleRepository.AsUnFilteredQueryable().AsNoTracking()
                .Select(r => RoleListDto.FromRole(r)).AsEnumerable();

            return TaskResult<IEnumerable<RoleListDto>>.Ok(roles);
        }

        public TaskResult UpdateRole(AddRoleDto model)
        {
            var role = _roleRepository.FirstOrDefault(n => n.Id == model.Id);
            if (role != null)
            {
                role.Name = model.Name;
                role.Description = model.Description;
                role.ConcurrencyStamp = model.ConcurrencyStamp;
                _roleRepository.Update(role);
                _roleRepository.SaveChanges();

                return TaskResult.Ok();
            }

            return TaskResult.Fail("Role not found");
        }
    }
}
