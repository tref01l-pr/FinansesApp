using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Dto;

public class LoginDto
{
    public string? UserName { get; set; } = string.Empty;
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;
    public NumberDto? Number { get; set; } = null;
    [Required]
    public string Password { get; set; } = string.Empty;
}