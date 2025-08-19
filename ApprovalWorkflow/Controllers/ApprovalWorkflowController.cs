using ApprovalSystem.Interfaces;
using ApprovalSystem.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalSystem.Controllers;

[Authorize]
[ApiController]
[Route("admin/approval-workflow")]
public class ApprovalWorkflowController : ControllerBase
{
    private readonly IWorkflow _workflowService;
    public ApprovalWorkflowController(IWorkflow workflowService)
    {
        _workflowService = workflowService;
    }

    [HttpGet("all-approvals")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult ListProcessTypes()
    {
        var result = _workflowService.GetApprovals();
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("approval-details")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult ApprovalDetails()
    {
        var result = _workflowService.GetApprovalDetails(5);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("approval-item")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult ApproveItem()
    {
        var result = _workflowService.ApproveItem();
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("reject-item")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult RejectItem()
    {
        var result = _workflowService.RejectItem(5);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("sendback-item")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public IActionResult SendBackItem(long itemId, long toStep)
    {
        var result = _workflowService.SendBackStep();
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

}
