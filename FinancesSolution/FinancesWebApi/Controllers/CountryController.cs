using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models.User.UserSettings;
using Microsoft.AspNetCore.Mvc;

namespace FinancesWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController(ICountryPhoneNumberRepository countryPhoneNumberRepository, IMapper mapper) : ControllerBase
{
    [HttpGet("get-all-country-codes")]
    public IActionResult GetAllCountriesPhoneNumber()
    {
        var countryPhoneNumbers = countryPhoneNumberRepository.GetCountryPhoneNumbers();
        if (countryPhoneNumbers == null)
        {
            ModelState.AddModelError("", "No Countries");
            return StatusCode(500, ModelState);
        }
        
        var countryPhoneNumbersDto = mapper.Map<ICollection<CountryPhoneNumber>, ICollection<NumberWithMaskDto>>(countryPhoneNumbers);

        return Ok(countryPhoneNumbersDto);
    }
}