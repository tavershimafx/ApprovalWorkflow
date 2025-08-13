using ApprovalSystem.Extensions;
using ApprovalSystem.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalSystem.Controllers;

[ApiController]
[Authorize]
[Route("roles")]
public class RolesController : ControllerBase
{

    public RolesController()
    {

    }

    [HttpGet]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    public IActionResult Roles()
    {
        return Ok();
    }
}