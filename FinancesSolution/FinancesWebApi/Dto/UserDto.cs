using FinancesWebApi.Models;

namespace FinancesWebApi.Dto;

public class UserDto
{
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    public bool PhoneNumberConfirmed { get; set; } = false;
}