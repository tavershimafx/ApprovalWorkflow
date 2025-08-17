using ApprovalSystem.Dtos;
using ApprovalSystem.Services;
using ApprovalSystem.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalSystem.Controllers;

[ApiController]
[Authorize]
[Route("admin/roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("nf-all-roles")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult AdminAllRoles()
    {
        var roles = _roleService.GetRolesUnfiltered();
        return roles.Succeeded ? Ok(roles) : BadRequest(roles);
    }

    [HttpGet("all-roles")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult Roles()
    {
        var roles = _roleService.GetRoles();
        return roles.Succeeded ? Ok(roles) : BadRequest(roles);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult CreateRole(AddRoleDto model)
    {
        if (ModelState.IsValid)
        {
            var roleAdd = _roleService.AddRole(model);
            return roleAdd.Succeeded ? Ok(roleAdd) : BadRequest(roleAdd);
        }
        
        return BadRequest(TaskResult.Fail(ModelState.SelectMany(n => n.Value.Errors.Select(k => k.ErrorMessage))));
    }

    [HttpPost("update")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult UpdateRole(AddRoleDto model)
    {
        if (ModelState.IsValid)
        {
            var roleUpdate = _roleService.UpdateRole(model);
            return roleUpdate.Succeeded ? Ok(roleUpdate) : BadRequest(roleUpdate);
        }

        return BadRequest(TaskResult.Fail(ModelState.SelectMany(n => n.Value.Errors.Select(k => k.ErrorMessage))));
    }
}