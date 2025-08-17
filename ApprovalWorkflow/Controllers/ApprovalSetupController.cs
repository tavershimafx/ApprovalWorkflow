using ApprovalSystem.Dtos;
using ApprovalSystem.Interfaces;
using ApprovalSystem.Models;
using ApprovalSystem.Services;
using ApprovalSystem.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalSystem.Controllers;

[ApiController]
[Authorize]
[Route("admin/approval-setup")]
public class ApprovalSetupController : ControllerBase
{
    private readonly IApprovalSetup _approvaService;
    public ApprovalSetupController(IApprovalSetup approvaService)
    {
        _approvaService = approvaService;
    }

    [HttpGet("all-types")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult ListProcessTypes()
    {
        var result = _approvaService.GetApprovalTypes();
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("types-details")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult TypeDetails(long id)
    {
        var result = _approvaService.GetApprovalType(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("update-steps")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult UpdateSteps(long id, [FromBody]IEnumerable<ApprovalStepDto> steps)
    {
        if (ModelState.IsValid)
        {
            var result = _approvaService.UpdateSteps(id, steps);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        return BadRequest(TaskResult.Fail(ModelState.SelectMany(n => n.Value.Errors.Select(k => k.ErrorMessage))));
    }

    [HttpDelete("delete-steps")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult DeleteSteps(long id, [FromBody] IEnumerable<long> steps)
    {
        if (ModelState.IsValid)
        {
            var result = _approvaService.DeleteSteps(id, steps);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        return BadRequest(TaskResult.Fail(ModelState.SelectMany(n => n.Value.Errors.Select(k => k.ErrorMessage))));
    }

    [HttpGet("step-rules")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    public IActionResult StepRules()
    {
        return Ok(Enum.GetNames(typeof(ApprovalStepRule)).Select(x => new { Text = x, Value = x }));
    }
    [HttpGet("entity-status")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    public IActionResult EntityStatus()
    {
        return Ok(Enum.GetNames(typeof(EntityStatus)).Select(x => new { Text = x, Value = x }));
    }
}
