using System.Security.Claims;
using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models.User;
using FinancesWebApi.Models.User.UserSettings;
using FinancesWebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = Twilio.TwiML.Voice.Task;

namespace FinancesWebApi.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IUserSettingsRepository userSettingsRepository,
    IPasswordSecurityService passwordSecurityService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("get-user-info")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUserInfo()
    {
        if (!int.TryParse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            ModelState.AddModelError("", "Incorrect accessToken");
            return Unauthorized(ModelState);
        }

        User? user = await userRepository.GetUserAsync(userId);
        if (user == null)
        {
            ModelState.AddModelError("", "Incorrect accessToken");
            return Unauthorized(ModelState);
        }

        UserSettings? userSettings = await userSettingsRepository.GetUserSettingsByUserIdAsync(userId);
        if (userSettings == null)
        {
            ModelState.AddModelError("", "Incorrect accessToken");
            return Unauthorized(ModelState);
        }

        List<UserRole> userRoles = await userRoleRepository.GetRolesByUserIdAsync(userId);
        string[] roles = userRoles.Select(userRole => userRole.Role.Name).ToArray();

        var userDto = mapper.Map<UserDto>(user);
        var userSettingsDto = mapper.Map<UserSettingsDto>(userSettings);
        userDto.Email = passwordSecurityService.EncodeEmail(userDto.Email);

        return Ok(new
        {
            userInfo = userDto,
            userRoles = roles,
            userSettingsInfo = userSettingsDto
        });
    }

    [Authorize]
    [HttpGet("testWithAuthorize")]
    public async Task<IActionResult> Geta()
    {
        var headers = HttpContext.Request.Headers;
        var result = headers.Authorization;
        var asdf = HttpContext.User.Claims;
        var asd = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        return Ok("Success");
    }
    
    [HttpGet("test")]
    public async Task<IActionResult> Getasdasd()
    {
        var headers = HttpContext.Request.Headers;
        var result = headers.Authorization;
        var asdf = HttpContext.User.Claims;
        var asd = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        return Ok("Success");
    }
}