using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserRepository userRepository, IMapper mapper, IJwtService jwtService)
    : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult Register([FromBody] RegisterDto registerDto)
    {
        if (registerDto == null)
            return BadRequest(ModelState);

        if (userRepository.IsEmailExists(registerDto.Email) ||
            userRepository.IsUserNameExists(registerDto.UserName))
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
        
        if (!userRepository.IsUserExists(userId))
            return BadRequest(ModelState);


        var user = userRepository.GetUser(userId);
        var userMap = mapper.Map<UserDto>(user);

        return Ok(userMap);
    }
}