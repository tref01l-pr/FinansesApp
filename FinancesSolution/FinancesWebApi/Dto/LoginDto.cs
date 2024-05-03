using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Dto;

public class LoginDto
{
    public string? UserName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public NumberDto? PhoneNumber { get; set; } = null;
    [Required]
    public string Password { get; set; } = string.Empty;
}