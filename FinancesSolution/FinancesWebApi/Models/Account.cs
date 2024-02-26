namespace FinancesWebApi.Models;

public class Account
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Comments { get; set; }
}