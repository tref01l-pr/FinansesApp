namespace FinancesWebApi.Models;

public class Account
{
    public int AccountId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Comments { get; set; } = null!;
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public User User { get; set; }
}