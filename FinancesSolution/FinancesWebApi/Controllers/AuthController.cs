using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using dotenv.net;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;
using FinancesWebApi.Models.User.UserSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wangkanai.Detection.Services;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserRepository userRepository, IUserDeviceRepository deviceRepository, IPhoneNumberService phoneNumberService, ICountryPhoneNumberRepository countryPhoneNumberRepository, IMapper mapper, IPasswordSecurityService passwordSecurityService, IJwtService jwtService, IEmailSender emailSender, IDetectionService detection)
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
        browserDetails.Add(Request.Headers["Accept-Language"].ToString());
        
        return Ok(browserDetails);
    }
    
    [HttpGet("GetUser"), Authorize(Roles = "user, admin")]
    public ActionResult<string> GetMyName()
    {
        try
        {
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
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        /*var email = new EmailAddressAttribute();
        if (!email.IsValid(registerDto.Email))
        {
            ModelState.AddModelError("", $"Incorrect Email address");
            return StatusCode(422, ModelState);
        }*/
        
        //TODO:implement defence from XSS attack
        
        if (userRepository.IsUserWithEmailExists(registerDto.Email) ||
            userRepository.IsUserWithUserNameExists(registerDto.UserName))
        {
            ModelState.AddModelError("", $"This User already exists");
            return StatusCode(422, ModelState);
        }
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        passwordSecurityService.CreatePasswordHash(registerDto.Password,
            out byte[] passwordHash,
            out byte[] passwordSalt);

        var user = new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            VerificationEmailToken = userRepository.GetRandomVerificationEmailToken(),
            VerificationEmailTokenExpires = DateTime.Now.AddDays(1)
        };
            
        if (!userRepository.CreateUser(user))
        {
            ModelState.AddModelError("", $"Something went wrong saving {user.UserName}");
            return StatusCode(500, ModelState);
        }
        
        //TODO: should be link to frontend
        await emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Open this link: {user.VerificationEmailToken}");
        
        return Ok("Successfully created");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        if (loginDto is { UserName: "", Email: "", Number: null } || loginDto.Password == string.Empty)
            return BadRequest("Empty inputs");

        try
        {
            var user = new User
            {
                UserName = null,
                Email = null,
                PasswordHash = null,
                PasswordSalt = null,
                UserRoles = null
            };
            
            if (!string.IsNullOrEmpty(loginDto.UserName))
                user = userRepository.GetUserByName(loginDto.UserName);

            if (!string.IsNullOrEmpty(loginDto.Email))
            {
                var email = new EmailAddressAttribute();
                if (!email.IsValid(loginDto.Email))
                {
                    ModelState.AddModelError("", $"Incorrect Email address");
                    return StatusCode(422, ModelState);
                }
                user = userRepository.GetUserByEmail(loginDto.Email);
            }

            if (loginDto.Number != null)
                user = userRepository.GetUserByNumber(loginDto.Number);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            
            if (!passwordSecurityService.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError("", "Password or UserName is incorrect");
                return StatusCode(401, ModelState);
            }
            
            if (!user.EmailConfirmed || user.VerifiedEmailAt == null)
            {
                ModelState.AddModelError("", "Email not verified!");
                return StatusCode(401, ModelState);
            }
            
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
        User? user = userRepository.GetUser(userId);
        
        if (user == null)
            return BadRequest("User doesn't exist");
        
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
        
            var deviceById = deviceRepository.GetDeviceById(deviceId);
            device = deviceRepository.GetDeviceByRefreshToken(refreshToken);
            
            if (deviceById == null || device == null || !deviceById.Token.Equals(device.Token))
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

    [HttpPost("verifyEmail/{token}")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var user = userRepository.GetUserByVerificationEmailToken(token);

        if (user == null)
            return BadRequest("Invalid Token");

        if (user.VerificationEmailTokenExpires < DateTime.Now)
            return BadRequest("Token is Expired");

        user.VerificationEmailToken = null;
        user.VerifiedEmailAt = DateTime.Now;
        user.VerificationEmailTokenExpires = null;
        user.EmailConfirmed = true;

        if (!userRepository.UpdateUser(user))
        {
            ModelState.AddModelError("", "Cannot update Refresh Token");
            return StatusCode(500, ModelState);
        }

        return Ok("Your Email was Confirmed :)");
    }

    
    //TODO:Make forgot with phone number
    [HttpPost("forgot-password-email")]
    public async Task<IActionResult> RecoverPasswordByEmail(string userEmail)
    {
        var email = new EmailAddressAttribute();
        if (!email.IsValid(userEmail))
        {
            ModelState.AddModelError("", $"Incorrect Email address");
            return StatusCode(422, ModelState);
        }

        var user = userRepository.GetUserByEmail(userEmail);
        if (user == null || !user.EmailConfirmed)
        {
            ModelState.AddModelError("", $"User not found");
            return StatusCode(404, ModelState);
        }
        
        user.PasswordResetToken = userRepository.GetRandomPasswordResetToken();
        user.PasswordResetTokenExpires = DateTime.Now.AddHours(1);

        if (!userRepository.UpdateUser(user))
        {
            ModelState.AddModelError("", "Cannot update Refresh Token");
            return StatusCode(500, ModelState);
        }

        //TODO: should be link to frontend
        await emailSender.SendEmailAsync(user.Email, $"Password Recovery", $"Open this link: {user.PasswordResetToken}");
        
        return Ok("Open link on your email");
    }

    [HttpPost("forgot-password-phone")]
    public async Task<IActionResult> RecoverPasswordByPhoneNumber(NumberDto numberDto)
    {
        var user = userRepository.GetUserByNumber(numberDto);
        if (user == null) 
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(404, ModelState);
        }

        var countryPhoneNumber = countryPhoneNumberRepository.GetCountryPhoneNumber(numberDto.CountryCode);

        if (countryPhoneNumber == null)
        {
            ModelState.AddModelError("", "Country Code not found");
            return StatusCode(404, ModelState);
        }
        
        user.PasswordResetToken = phoneNumberService.GenerateCode();
        user.PasswordResetTokenExpires = DateTime.Now.AddHours(1);

        if (!userRepository.UpdateUser(user))
        {
            ModelState.AddModelError("", "Cannot update User. Try later");
            return StatusCode(500, ModelState);
        }
        
        var smsModel = new SmsModel
        {
            To = countryPhoneNumber.DialCode + numberDto.Number,
            From = "+48732071811",
            Message = user.PasswordResetToken!
        };

        if (await phoneNumberService.SendSms(smsModel))
        {
            ModelState.AddModelError("", "Something went wrong while sending message");
            return StatusCode(500, ModelState);
        }
        return Ok("Success");
    }

    //TODO: Make reset-password optimised for every model of recovery(email, phone number)
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = userRepository.GetUserByPasswordResetToken(resetPasswordDto.Token);

        if (user == null || user.PasswordResetTokenExpires < DateTime.Now)
        {
            ModelState.AddModelError("", "Invalid token");
            return StatusCode(401, ModelState);
        }
        
        passwordSecurityService.CreatePasswordHash(resetPasswordDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
        
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        if (!userRepository.UpdateUser(user))
        {
            ModelState.AddModelError("", "Cannot update Refresh Token");
            return StatusCode(500, ModelState);
        }

        return Ok("Your password was updated :)");
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