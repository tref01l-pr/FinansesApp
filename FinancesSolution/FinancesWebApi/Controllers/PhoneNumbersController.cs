﻿using System.Security.Claims;
using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;
using FinancesWebApi.Models.User.UserSettings;
using FinancesWebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhoneNumbersController(IPhoneNumberRepository numberRepository, IUserRepository userRepository, IUserDeviceRepository userDeviceRepository, IPhoneNumberService phoneNumberService, IMapper mapper, IMaskConverter maskConverter) : ControllerBase
{
    [HttpGet("getCountriesNumbersWithMasks")]
    public IActionResult GetCountriesNumbersWithMasks()
    {
        var countryNumbers = mapper.Map<List<NumberWithMaskDto>>(numberRepository.GetCountryPhoneNumbers());
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(countryNumbers);
    }
    
    [Authorize]
    [HttpPost("createUserNumber")]
    public IActionResult CreateUserNumber([FromBody] NumberDto numberDto)
    {
        var jwt = Request.Cookies["jwt"];
        var header = HttpContext.Request.Headers["Authorization"];
        if (header != jwt)
        {
            return BadRequest("ERROR");
        }
        
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        if (numberRepository.IsNumberExists(numberDto.CountryCode, numberDto.Number))
        {
            ModelState.AddModelError("", "Number already exists");
            return StatusCode(409, ModelState);
        }

        User? user = userRepository.GetUser(userId);
        
        if (user == null)
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(404, ModelState);
        }

        var countryPhoneNumber = numberRepository.GetCountryPhoneNumber(numberDto.CountryCode);
        
        if (countryPhoneNumber == null)                        
        {
            ModelState.AddModelError("", $"No country with code {numberDto.CountryCode}");
            return StatusCode(404, ModelState);
        }

        if (!maskConverter.NumberIsValid(countryPhoneNumber.Mask, numberDto.Number))
        {
            ModelState.AddModelError("", "Incorrect number format");
            return StatusCode(422, ModelState);
        }
        
        /*var user = ValidateNumber(numberDto, userId, out var validationResult);
        if (user == null)
            return validationResult;*/

        //var numberMap = mapper.Map<UserPhoneNumber>(numberDto);
        //numberMap.User = user;

        user.VerificationPhoneNumberCode = phoneNumberService.GenerateCode();
        user.VerificationPhoneNumberCodeExpires = DateTime.Now.AddHours(1);
        
       //TODO: Send message to phone

        return Ok("Number was accepted");
    }

    [Authorize]
    [HttpPost("verify-user-phone-number")]
    public async Task<IActionResult> VerifyUserPhoneNumber([FromBody] VerifyNumberDto numberDto)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(401, ModelState);
        }
        
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        User? user = userRepository.GetUser(userId);
        Device? userDevice = userDeviceRepository.GetDeviceByRefreshToken(refreshToken);
        
        if (user == null || userDevice == null || user.Id != userDevice.UserId)
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(404, ModelState);
        }
        
        if (numberRepository.IsNumberExists(numberDto.CountryCode, numberDto.Number))
        {
            ModelState.AddModelError("", "Number already exists");
            return StatusCode(409, ModelState);
        }

        var countryPhoneNumber = numberRepository.GetCountryPhoneNumber(numberDto.CountryCode);
        
        if (countryPhoneNumber == null)                        
        {
            ModelState.AddModelError("", $"No country with code {numberDto.CountryCode}");
            return StatusCode(404, ModelState);
        }

        if (!maskConverter.NumberIsValid(countryPhoneNumber.Mask, numberDto.Number))
        {
            ModelState.AddModelError("", "Incorrect number format");
            return StatusCode(422, ModelState);
        }

        if (user.VerificationPhoneNumberCode != numberDto.Code ||
            user.VerificationPhoneNumberCodeExpires < DateTime.Now)
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(401, ModelState);
        }

        var userPhoneNumber = new UserPhoneNumber
        {
            Number = numberDto.Number,
            CountryPhoneNumber = countryPhoneNumber,
            User = user
        };

        if (!numberRepository.CreateUserNumber(userPhoneNumber))
        {
            ModelState.AddModelError("", "Cannot create Number");
            return StatusCode(500, ModelState);
        }

        return Ok("Your number was verified :)");
    }
    
    
    
    [Authorize]
    [HttpPost("send-verification-sms")]
    public IActionResult SendVerificationSms([FromBody] NumberDto numberDto)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(401, ModelState);
        }
        
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        User? user = userRepository.GetUser(userId);
        Device? userDevice = userDeviceRepository.GetDeviceByRefreshToken(refreshToken);
        
        if (user == null || userDevice == null || user.Id != userDevice.UserId)
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(404, ModelState);
        }

        var numberToUpdate = numberRepository.GetPhoneNumberWithUserId(userId);
        if (numberToUpdate == null)
        {
            ModelState.AddModelError("", "User number not found");
            return StatusCode(404, ModelState);
        }
        
        //TODO: Send code to user

        user.ResetPhoneNumberCode = phoneNumberService.GenerateCode();
        user.ResetPhoneNumberCodeExpires = DateTime.Now.AddHours(1);

        if (!userRepository.UpdateUser(user))
        {
            ModelState.AddModelError("", "Failed to update user number");
            return StatusCode(500, ModelState);
        }

        return Ok("Enter code from your phone");
    }

    [Authorize]
    [HttpGet("check-reset-phone-number-code")]
    public IActionResult CheckResetUserPhoneNumber(string code)
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = userRepository.GetUser(userId);
        if (user == null)
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(401, ModelState);
        }

        if (user.ResetPhoneNumberCode == code || user.ResetPhoneNumberCodeExpires < DateTime.Now)
        {
            ModelState.AddModelError("", "Code is incorrect");
            return StatusCode(401, ModelState);
        }
        
        return Ok("Your number was correct");
    }
    
    [Authorize]
    [HttpPost("send-sms-to-new-number")]
    public async Task<IActionResult> SendSmsToNewNumber([FromBody] NumberDto numberDto, string codeToReset)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(codeToReset))
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(401, ModelState);
        }
        
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        User? user = userRepository.GetUser(userId);
        Device? userDevice = userDeviceRepository.GetDeviceByRefreshToken(refreshToken);
        
        if (user == null || 
            userDevice == null || 
            user.Id != userDevice.UserId ||
            user.ResetPhoneNumberCode != codeToReset ||
            user.ResetPhoneNumberCodeExpires < DateTime.Now)
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(404, ModelState);
        }
        
        if (numberRepository.IsNumberExists(numberDto.CountryCode, numberDto.Number))
        {
            ModelState.AddModelError("", "Number already exists");
            return StatusCode(409, ModelState);
        }

        var countryPhoneNumber = numberRepository.GetCountryPhoneNumber(numberDto.CountryCode);
        
        if (countryPhoneNumber == null)                        
        {
            ModelState.AddModelError("", $"No country with code {numberDto.CountryCode}");
            return StatusCode(404, ModelState);
        }

        if (!maskConverter.NumberIsValid(countryPhoneNumber.Mask, numberDto.Number))
        {
            ModelState.AddModelError("", "Incorrect number format");
            return StatusCode(422, ModelState);
        }
        
        user.VerificationPhoneNumberCode = phoneNumberService.GenerateCode();
        user.VerificationPhoneNumberCodeExpires = DateTime.Now.AddHours(1);

        if (userRepository.UpdateUser(user))
        {
            ModelState.AddModelError("", "Failed to update user number");
            return StatusCode(500, ModelState);
        }
        
        //TODO: send sms
        return Ok("Enter your code");
    }
    
    [Authorize]
    [HttpPost("update-user-phone-number")]
    public async Task<IActionResult> UpdateUserPhoneNumber([FromBody] VerifyNumberDto numberDto, string codeToReset)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(codeToReset) || string.IsNullOrEmpty(numberDto.Code))
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(401, ModelState);
        }
        
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        User? user = userRepository.GetUser(userId);
        Device? userDevice = userDeviceRepository.GetDeviceByRefreshToken(refreshToken);
        
        if (user == null || 
            userDevice == null || 
            user.Id != userDevice.UserId || 
            user.ResetPhoneNumberCode != codeToReset || 
            user.VerificationPhoneNumberCode != numberDto.Code || 
            user.ResetEmailTokenExpires < DateTime.Now ||
            user.VerificationPhoneNumberCodeExpires < DateTime.Now)
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(401, ModelState);
        }
        
        if (numberRepository.IsNumberExists(numberDto.CountryCode, numberDto.Number))
        {
            ModelState.AddModelError("", "Number already exists");
            return StatusCode(409, ModelState);
        }

        var countryPhoneNumber = numberRepository.GetCountryPhoneNumber(numberDto.CountryCode);
        
        if (countryPhoneNumber == null)                        
        {
            ModelState.AddModelError("", $"No country with code {numberDto.CountryCode}");
            return StatusCode(404, ModelState);
        }

        if (!maskConverter.NumberIsValid(countryPhoneNumber.Mask, numberDto.Number))
        {
            ModelState.AddModelError("", "Incorrect number format");
            return StatusCode(422, ModelState);
        }

        var userPhoneNumber =  numberRepository.GetPhoneNumberWithUserId(userId);

        if (userPhoneNumber == null)
        {
            ModelState.AddModelError("", "Number not found");
            return StatusCode(404, ModelState);
        }

        userPhoneNumber.Number = numberDto.Number;
        userPhoneNumber.CountryPhoneNumber = countryPhoneNumber;

        if (!numberRepository.UpdateUserNumber(userPhoneNumber))
        {
            ModelState.AddModelError("", "Cannot update Number");
            return StatusCode(500, ModelState);
        }

        user.ResetPhoneNumberCodeExpires = null;
        user.ResetPhoneNumberCode = null;
        user.VerificationPhoneNumberCode = null;
        user.VerificationPhoneNumberCodeExpires = null;
        user.VerifiedPhoneNumberAt = DateTime.Now;

        if (!userRepository.UpdateUser(user))
        {
            ModelState.AddModelError("", "Cannot update User");
            return StatusCode(500, ModelState);
        }

        return Ok("Your number was verified :)");
    }
    
    
    
    private void UpdateUserPhoneNumber(UserPhoneNumber userPhoneNumber, NumberDto numberDto)
    {
        userPhoneNumber.Number = numberDto.Number;
        userPhoneNumber.CountryCode = numberDto.CountryCode;
    }

    
}