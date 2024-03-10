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

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserRepository userRepository, IUserDeviceRepository deviceRepository, IMapper mapper, IJwtService jwtService)
    : ControllerBase
{
    
    
    /*public class BrowserDetails
    {
        public string UserAgent { get; set; }
        public Dictionary<string, string> HttpHeaders { get; set; }
        public string Referrer { get; set; }
        public string IPAddress { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; set; }
    }

    [HttpGet("getBrowserDetails")]
    public BrowserDetails GetBrowserDetails()
    {
        var userAgent = Request.Headers["User-Agent"].ToString();
        var uaParser = Parser.GetDefault();
        ClientInfo clientInfo = uaParser.Parse(userAgent);

        var browserDetails = new BrowserDetails
        {
            UserAgent = userAgent,
            HttpHeaders = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            Referrer = Request.Headers["Referer"].ToString(),
            IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
            Browser = clientInfo.UA.Family,
            BrowserVersion = clientInfo.UA.Major,
            OperatingSystem = clientInfo.OS.Family
        };

        return browserDetails;
    }*/

    
    /*[HttpGet("getInfo")]
    public ActionResult GetInfo()
    {
        string userDetails = 
    $"User Agent: {Request.UserAgent}\n\n" +
    $"HTTP Headers: \n{string.Join("\n", Request.Headers.AllKeys.Select(key => $"{key}: {Request.Headers[key]}"))}\n\n" +
    $"Referrer: {Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "Unknown"}\n\n" +
    $"MIME Types: \n{string.Join(", ", Request.Browser.MimeTypes)}\n\n" +
    $"Client Hints: \n[Not available on server]\n\n" +
    $"Navigator Platform: {Request.Browser.Platform}\n\n" +
    $"Operating System: {Environment.OSVersion}\n\n" +
    $"Screen Size: {Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}\n\n" +
    $"IP Address: {Request.UserHostAddress}\n\n" +
    $"IP Address Location: [External service needed]\n\n" +
    $"Local IP Address: [Local network configuration]\n\n" +
    $"Using TOR: [Not easily detectable on server]\n\n" +
    $"WiFi: [Not available on server]\n\n" +
    $"ISP: [External service needed]\n\n" +
    $"ISP filtering outgoing network ports: [Not easily detectable on server]\n\n" +
    $"Internet Speed: [External service needed]\n\n" +
    $"Browser Versions: \n" +
    $"- Chrome: [Extract from User Agent]\n" +
    $"- Firefox: [Extract from User Agent]\n" +
    $"- Safari: [Extract from User Agent]\n" +
    $"- Internet Explorer: [Extract from User Agent]\n" +
    $"- Edge: [Extract from User Agent]\n" +
    $"- Opera: [Extract from User Agent]\n" +
    $"- Vivaldi: [Extract from User Agent]\n" +
    $"- Yandex Browser: [Extract from User Agent]\n\n" +
    $"Operating System Versions: \n" +
    $"- Chrome OS: [Extract from User Agent]\n" +
    $"- macOS: [Extract from User Agent]\n" +
    $"- iOS: [Extract from User Agent]\n" +
    $"- Windows: [Extract from User Agent]\n" +
    $"- Android: [Extract from User Agent]\n";

// Теперь вы можете использовать переменную userDetails для вывода информации или ее отображения где-либо еще.

    }*/
    
    
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

            string token = jwtService.Generate(user);
            
            Response.Cookies.Append("jwt", "Bearer " + token, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(2)
            });

            RefreshToken refreshToken = jwtService.GenerateRefreshToken();
            
            SetRefreshToken(refreshToken);

            //TODO: сделать проверку на юзер девайс, если такой есть, то обновить данные, если нет, то создать новый
            //userRepository.UpdateUserRefreshToken(user, refreshToken);

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
    public async Task<ActionResult<string>> RefreshToken(string? jwtToken)
    {
        string? refreshToken = Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(jwtToken))
            return Unauthorized("Cannot be Refreshed");

        //TODO: Сделать проверку, если нету jwt токена, то сделать проверку данных компьютера и только после всей проверки апдейтнуть accessToken и refreshToken
        /*if (string.IsNullOrEmpty(jwtToken))
        {
            
        }*/

        JwtSecurityToken token = jwtService.Verify(jwtToken);
        
        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null || int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized("Cannot be refreshed");                                  //incorrect access token
        
        var device = deviceRepository.GetDeviceByUserId(userId);
        
        if (device == null || !device.Token.Equals(deviceRepository.GetDeviceByRefreshToken(refreshToken).Token))
            return Unauthorized("Invalid Refresh token");                                //fake access token
        
        if (device.TokenExpires < DateTime.Now)
            return Unauthorized("Token expired.");
        

        string newToken = jwtService.Generate(userRepository.GetUser(userId)!);

        var newRefreshToken = jwtService.GenerateRefreshToken();

        if (deviceRepository.UpdateRefreshToken(device, newRefreshToken))
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