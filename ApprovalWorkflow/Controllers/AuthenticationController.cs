using ApprovalSystem.Dtos;
using ApprovalSystem.Models;
using ApprovalSystem.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalSystem.Controllers;

[ApiController]
[AllowAnonymous]
[Route("identity")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthenticationController(ILogger<AuthenticationController> logger, UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody]LoginDto model)
    {
        if (ModelState.IsValid)
        {
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Ok(TaskResult.Ok());
            }

            return BadRequest(TaskResult.Fail("Invalid login attempt"));
        }

        return BadRequest(TaskResult.Fail(ModelState.SelectMany(n => n.Value.Errors.Select(k => k.ErrorMessage))));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(TaskResult<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");
                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _userManager.ConfirmEmailAsync(user, code);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(TaskResult.Ok());
            }

            return BadRequest(TaskResult.Fail(result.Errors.Select(n => n.Description)));
        }

        return BadRequest(TaskResult.Fail(ModelState.SelectMany(n => n.Value.Errors.Select(k => k.ErrorMessage))));
    }
}
