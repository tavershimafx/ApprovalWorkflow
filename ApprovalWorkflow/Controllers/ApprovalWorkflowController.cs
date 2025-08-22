using ApprovalSystem.Dtos;
using ApprovalSystem.Extensions;
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
    private readonly IHttpContextAccessor _context;

    public ApprovalWorkflowController(IWorkflow workflowService, IHttpContextAccessor context)
    {
        _workflowService = workflowService;
        _context = context;
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
    public IActionResult ApprovalDetails(long id)
    {
        var result = _workflowService.GetApprovalDetails(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("approval-item")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveItem([FromBody] WorkflowDto model)
    {
        var currentUser = (await _context.GetCurrentUser()).Id;
        var result = _workflowService.ApproveItem(model.ItemId, model.Comment, currentUser);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("reject-item")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectItem([FromBody] WorkflowDto model)
    {
        var currentUser = (await _context.GetCurrentUser()).Id;
        var result = _workflowService.RejectItem(model.ItemId, model.Comment, currentUser);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("sendback-item")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendBackItem([FromBody] WorkflowDto model)
    {
        var currentUser = (await _context.GetCurrentUser()).Id;
        var result = _workflowService.SendBackStep(model.ItemId, model.Comment, currentUser);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

}
