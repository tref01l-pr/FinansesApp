using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wangkanai.Detection.Services;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserRepository userRepository, IUserDeviceRepository deviceRepository, IMapper mapper, IJwtService jwtService, IDetectionService detection)
    : ControllerBase
{
    public class BrowserDetails
    {
        public string State { get; set; } = string.Empty;
        public void Add(string info) => State += info + " - ";
    }

    [HttpGet("getBrowserDetails")]
    public IActionResult GetBrowserDetails()
    {
        
        /*string browser_information = detection.Browser.Name.ToString() +
                                     detection.Browser.Version + detection.Device.Type + detection.;*/
        
        BrowserDetails browserDetails = new BrowserDetails();
        
        browserDetails.Add(detection.Browser.Name.ToString());
        browserDetails.Add(detection.Browser.Version.ToString());
        browserDetails.Add(detection.Device.Type.ToString());
        browserDetails.Add(detection.Crawler.IsCrawler.ToString());
        browserDetails.Add(detection.Crawler.Version.ToString());
        browserDetails.Add(detection.Crawler.Name.ToString());
        browserDetails.Add(detection.Engine.Name.ToString());
        browserDetails.Add(detection.Platform.Name.ToString());
        browserDetails.Add(detection.Platform.Version.ToString());
        browserDetails.Add(detection.Platform.Processor.ToString());
        browserDetails.Add(HttpContext.Connection.RemoteIpAddress.ToString());
        
        return Ok(browserDetails);
    }

    
    
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
            ModelState.AddModelError("", $"Something went wrong {e}");
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
            
            RefreshToken refreshToken = jwtService.GenerateRefreshToken();

            Device device = new Device
            {
                Ip = HttpContext.Connection.RemoteIpAddress!.ToString(),
                BrowserName = detection.Browser.Name.ToString(),
                BrowserVersion = detection.Browser.Version.ToString(),
                PlatformName = detection.Platform.Name.ToString(),
                PlatformVersion = detection.Platform.Version.ToString(),
                PlatformProcessor = detection.Platform.Processor.ToString(),
                Token = refreshToken.Token,
                TokenCreated = refreshToken.Created,
                TokenExpires = refreshToken.Expires,
                User = user
            };

            if (!deviceRepository.CreateDevice(device))
            {
                ModelState.AddModelError("", $"Cannot create Device");
                return StatusCode(500, ModelState);
            }
            
            //TODO: Can be incorrect value if it will be used by many people?
            string token = jwtService.Generate(user, device.Id);
            
            SetRefreshToken(refreshToken);

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
    public async Task<ActionResult<string>> RefreshToken(string? jwtToken = null)
    {
        string? refreshToken = Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Cannot be Refreshed");

        int userId = 0;
        Device? device = null;

        //TODO: if use check in two places (when we have jwt and don't) jwt stopping be necessary !
        if (string.IsNullOrEmpty(jwtToken))
        {
            device = deviceRepository.GetDeviceByRefreshToken(refreshToken);
            if (device == null)
                return Unauthorized("No RefreshToken");

            userId = device.UserId;

            if (device.BrowserName != detection.Browser.Name.ToString() ||
                device.BrowserVersion != detection.Browser.Version.ToString() ||
                device.PlatformName != detection.Platform.Name.ToString() ||
                device.PlatformVersion != detection.Platform.Version.ToString() ||
                device.PlatformProcessor != detection.Platform.Processor.ToString() ||
                device.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Incorrect RefreshToken");
            }
        }
        else
        {
            JwtSecurityToken token = jwtService.Verify(jwtToken);
            
            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
                return Unauthorized("Cannot be refreshed. No userId in Claims");                                  //incorrect access token

            var deviceIdClaim = token.Claims.FirstOrDefault(c => c.Type == "deviceId");
            if (deviceIdClaim == null || !int.TryParse(deviceIdClaim.Value, out int deviceId))
                return Unauthorized("Cannot be refreshed. No deviceId in Claims");                                  //incorrect access token
        
            device = deviceRepository.GetDeviceById(deviceId);
        
            if (device == null || !device.Token.Equals(deviceRepository.GetDeviceByRefreshToken(refreshToken).Token))
                return Unauthorized("Invalid Refresh token");                                //fake access token
        
            if (device.TokenExpires < DateTime.Now)
                return Unauthorized("Token expired.");
        }
        
        string newToken = jwtService.Generate(userRepository.GetUser(userId)!, device.Id);

        var newRefreshToken = jwtService.GenerateRefreshToken();

        if (!deviceRepository.UpdateRefreshToken(device, newRefreshToken))
        {
            ModelState.AddModelError("", "Cannot update Refresh Token");
            return StatusCode(500, ModelState);
        }
        
        SetRefreshToken(newRefreshToken);
        
        return Ok(newToken);
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