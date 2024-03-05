using FinancesWebApi.Models;

namespace FinancesWebApi.Dto;

public class UserDto
{
    public int Id { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    public bool PhoneNumberConfirmed { get; set; } = false;
}