using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Dto;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    [Required, Compare("Password")]
    public string PasswordConfirm { get; set; } = string.Empty;
}