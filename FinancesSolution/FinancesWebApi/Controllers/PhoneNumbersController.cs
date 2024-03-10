using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User.UserSettings;
using FinancesWebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhoneNumbersController(IPhoneNumberRepository numberRepository, IUserRepository userRepository, IMapper mapper, IMaskConverter maskConverter) : ControllerBase
{
    [HttpGet("getCountriesNumbersWithMasks")]
    public IActionResult GetCountriesNumbersWithMasks()
    {
        var countryNumbers = mapper.Map<List<NumberWithMaskDto>>(numberRepository.GetCountryPhoneNumbers());
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(countryNumbers);
    }

    [HttpPost("createUserNumber")]
    public IActionResult CreateUserNumber([FromBody] NumberDto numberDto)
    {
        if (!ValidateNumber(numberDto, out var validationResult))
            return validationResult;

        var numberMap = mapper.Map<UserPhoneNumber>(numberDto);
        
        if (!numberRepository.CreateUserNumber(numberMap))
        {
            ModelState.AddModelError("", "Something went wrong");
            return StatusCode(500, ModelState);
        }

        return Ok("Number was accepted");
    }
    
    [HttpPost("updateUserNumber")]
    public IActionResult UpdateUserNumber([FromBody] NumberDto numberDto)
    {
        if (!ValidateNumber(numberDto, out var validationResult))
        {
            return validationResult;
        }

        var numberToUpdate = numberRepository.GetPhoneNumberWithUserId(numberDto.UserId);
        if (numberToUpdate == null)
        {
            ModelState.AddModelError("", "User number not found");
            return StatusCode(404, ModelState);
        }

        UpdateUserPhoneNumber(numberToUpdate, numberDto);

        if (!numberRepository.UpdateUserNumber(numberToUpdate))
        {
            ModelState.AddModelError("", "Failed to update user number");
            return StatusCode(500, ModelState);
        }

        return Ok("Number was updated");
    }

    private bool ValidateNumber(NumberDto numberDto, out IActionResult validationResult)
    {
        if (numberRepository.IsNumberExists(numberDto.CountryCode, numberDto.Number.ToString()))
        {
            validationResult = StatusCode(409, "Number already exists");
            return false;
        }

        if (!userRepository.IsUserWithIdExists(numberDto.UserId))
        {
            validationResult = StatusCode(404, "User not found");
            return false;
        }

        if (!numberRepository.IsCountryCodeExists(numberDto.CountryCode))                        
        {
            validationResult = StatusCode(404, $"No country with code {numberDto.CountryCode}");
            return false;
        }

        //Add Verification for user with jwt
        
        CountryPhoneNumber countryPhoneNumber = numberRepository.GetCountryPhoneNumber(numberDto.CountryCode);
        if (!maskConverter.NumberIsValid(countryPhoneNumber.Mask, numberDto.Number))
        {
            validationResult = StatusCode(422, "Incorrect number format");
            return false;
        }

        validationResult = null;
        return true;
    }

    private void UpdateUserPhoneNumber(UserPhoneNumber userPhoneNumber, NumberDto numberDto)
    {
        userPhoneNumber.Number = numberDto.Number;
        userPhoneNumber.CountryCode = numberDto.CountryCode;
    }

    
}