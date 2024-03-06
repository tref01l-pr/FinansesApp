using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using FinancesWebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhoneNumbersController(IPhoneNumberRepository numberRepository, IUserRepository userRepository, IMapper mapper, IMaskConverter maskConverter) : ControllerBase
{
    [HttpGet("getCountryNumbersWithMasks")]
    public IActionResult GetCountryNumbersWithMasks()
    {
        var countryNumbers = mapper.Map<List<NumberWithMaskDto>>(numberRepository.GetCountryPhoneNumbers());
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(countryNumbers);
    }

    [HttpPost("createUserNumber")]
    public IActionResult CreateUserNumber([FromBody] NumberDto numberDto)
    {
        if (numberRepository.IsNumberExists(numberDto.CountryCode, numberDto.Number.ToString()))
        {
            ModelState.AddModelError("", $"Number already exists");
            return StatusCode(409, ModelState);
        }

        if (!userRepository.IsUserWithIdExists(numberDto.UserId))
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(404, ModelState);
        }
        
        if (!numberRepository.IsCountryCodeExists(numberDto.CountryCode))
        {
            ModelState.AddModelError("", $"No country with this code");
            return StatusCode(404, ModelState);
        }

        CountryPhoneNumber countryPhoneNumber = numberRepository.GetCountryPhoneNumber(numberDto.CountryCode);

        if (!maskConverter.NumberIsValid(countryPhoneNumber.Mask, numberDto.Number))
        {
            ModelState.AddModelError("", $"Incorrect number");
            return StatusCode(422, ModelState);
        }

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
        if (numberRepository.IsNumberExists(numberDto.CountryCode, numberDto.Number.ToString()))
        {
            ModelState.AddModelError("", $"Number exists");
            return StatusCode(409, ModelState);
        }

        if (!userRepository.IsUserWithIdExists(numberDto.UserId))
        {
            ModelState.AddModelError("", "User not found");
            return StatusCode(404, ModelState);
        }

        /*if (userRepository.GetUser(numberDto.UserId)!.Id != numberDto.UserId)          //Should compare with jwt tokens
        {
            ModelState.AddModelError("", "Something went wrong");
            return StatusCode(404, ModelState);
        }*/
        
        if (!numberRepository.IsCountryCodeExists(numberDto.CountryCode))
        {
            ModelState.AddModelError("", $"No country with this code");
            return StatusCode(404, ModelState);
        }

        CountryPhoneNumber countryPhoneNumber = numberRepository.GetCountryPhoneNumber(numberDto.CountryCode);

        if (!maskConverter.NumberIsValid(countryPhoneNumber.Mask, numberDto.Number))
        {
            ModelState.AddModelError("", $"Incorrect number");
            return StatusCode(422, ModelState);
        }

        var numberMap = mapper.Map<UserPhoneNumber>(numberDto);
        
        if (!numberRepository.UpdateUserNumber(numberMap))
        {
            ModelState.AddModelError("", "Something went wrong");
            return StatusCode(500, ModelState);
        }

        return Ok("Number was accepted");
    }
    
}