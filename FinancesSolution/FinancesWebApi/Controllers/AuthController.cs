using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserRepository userRepository, IMapper mapper, IJwtService jwtService)
    : ControllerBase
{
    [HttpGet, Authorize(Roles = "user, admin")]
    public ActionResult<string> GetMyName()
    {
        try
        {
            var jwt = Request.Cookies["jwt"];
            var header = HttpContext.Request.Headers["Authorization"];
            if (header != jwt)
            {
                return BadRequest("ERROR");
            }
        
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
            return Ok(userRepository.GetUser(userId));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Something went wrong");
            return StatusCode(500, ModelState);
        }
        
    }
    
    //var userName = User?.Identity?.Name;
    //var roleClaims = User?.FindAll(ClaimTypes.Role);
    //var roles = roleClaims?.Select(c => c.Value).ToList();
    //var roles2 = User?.Claims
    //    .Where(c => c.Type == ClaimTypes.Role)
    //    .Select(c => c.Value)
    //    .ToList();
    //return Ok(new { userName, roles, roles2 });
    
    [HttpPost("register")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult Register([FromBody] RegisterDto registerDto)
    {
        if (registerDto == null)
            return BadRequest(ModelState);

        if (userRepository.IsUserWithEmailExists(registerDto.Email) ||
            userRepository.IsUserWithUserNameExists(registerDto.UserName))
        {
            ModelState.AddModelError("", $"This User already exists");
            return StatusCode(422, ModelState);
        }

        var email = new EmailAddressAttribute();
        if (!email.IsValid(registerDto.Email))
        {
            ModelState.AddModelError("", $"Incorrect Email address");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userMap = mapper.Map<User>(registerDto);

        if (!userRepository.CreateUser(userMap))
        {
            ModelState.AddModelError("", $"Something went wrong saving {userMap.UserName}");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Successfully created");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        if (loginDto is { UserName: "", Email: "", Number: null } || loginDto.Password == string.Empty)
            return BadRequest(ModelState);

        try
        {
            var user = new User
            {
                UserName = null,
                Email = null,
                Password = null,
                UserRoles = null
            };
            
            if (loginDto.UserName != string.Empty)
                user = userRepository.GetUserByName(loginDto.UserName);
            
            if (loginDto.Email != string.Empty)
                user = userRepository.GetUserByEmail(loginDto.Email);

            if (loginDto.Number != null)
                user = userRepository.GetUserByNumber(loginDto.Number);
            

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new Exception("Invalid UserName or Password");

            string token = jwtService.Generate(user);
            
            Response.Cookies.Append("jwt", "Bearer " + token, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(2)
            });

            RefreshToken refreshToken = jwtService.GenerateRefreshToken();
            
            SetRefreshToken(refreshToken);

            userRepository.UpdateUserRefreshToken(user, refreshToken);

            return Ok(token);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", e.Message);
            return StatusCode(500, ModelState);
        }
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(200, Type = typeof(User))]
    [ProducesResponseType(400)]
    public IActionResult GetUser(int userId)
    {
        var users = userRepository.GetUsers();
        
        if (!userRepository.IsUserWithIdExists(userId))
            return BadRequest(ModelState);


        var user = userRepository.GetUser(userId);
        var userMap = mapper.Map<UserDto>(user);

        return Ok(userMap);
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult<string>> RefreshToken(int userId)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var user = userRepository.GetUser(userId);

        if (user == null || !user.RefreshToken.Equals(refreshToken))
        {
            return Unauthorized("Invalid Refresh token");
        }
        
        if (user.TokenExpires < DateTime.Now)
        {
            return Unauthorized("Token expired.");
        }

        string token = jwtService.Generate(user);

        var newRefreshToken = jwtService.GenerateRefreshToken();

        userRepository.UpdateUserRefreshToken(user, newRefreshToken);

        return Ok(token);
    }

    private void SetRefreshToken(RefreshToken newRefreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires
        };
        
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
    }
}