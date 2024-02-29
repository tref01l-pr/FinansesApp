using FinancesWebApi.Models;

namespace FinancesWebApi.Dto;

public class UserDto
{
    public int Id { get; set; }
    public int UserSettingsId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    public string? PhoneNumber { get; set; } = null;
    public bool PhoneNumberConfirmed { get; set; } = false;
    public DateTime DateOfRegistration { get; set; }
    public UserSettings UserSettings { get; set; }
}