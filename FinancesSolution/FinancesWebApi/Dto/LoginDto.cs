namespace FinancesWebApi.Dto;

public class LoginDto
{
    public string? UserName { get; set; } = String.Empty;
    public string? Email { get; set; } = String.Empty;
    public NumberDto? Number { get; set; } = null;
    public required string Password { get; set; }
}