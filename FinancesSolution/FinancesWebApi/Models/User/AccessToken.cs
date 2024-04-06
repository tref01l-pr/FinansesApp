namespace FinancesWebApi.Models.User;

public class AccessToken
{
    public required string Token { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Expires { get; set; }
}