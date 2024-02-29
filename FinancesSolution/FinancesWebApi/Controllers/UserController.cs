using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateUser([FromBody] UserDto userDto)
    {
        if (userDto == null)
            return BadRequest(ModelState);

        if (_userRepository.GetUserByEmail(userDto.Email) != null ||
            _userRepository.GetUserByName(userDto.UserName) != null)
        {
            ModelState.AddModelError("", $"This User already exists");
            return StatusCode(422, ModelState);
        }

        var email = new EmailAddressAttribute();
        if (!email.IsValid(userDto.Email))
            return BadRequest(ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userMap = _mapper.Map<User>(userDto);

        userMap = new User()
        {
            UserName = "test",
            Email = "test@test.com",
            Password = "test1"
        };

        if (!_userRepository.CreateUser(userMap))
        {
            ModelState.AddModelError("", $"Something went wrong saving {userMap.UserName}");
            return StatusCode(500, ModelState);
        }
        
        
        
        return Ok("Successfully created");
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(200, Type = typeof(User))]
    [ProducesResponseType(400)]
    public IActionResult GetUser(int userId)
    {
        var users = _userRepository.GetUsers();
        
        if (!_userRepository.UserExists(userId))
            return BadRequest(ModelState);


        var user = _userRepository.GetUser(userId);
        var userMap = _mapper.Map<UserDto>(user);

        return Ok(userMap);
    }
}