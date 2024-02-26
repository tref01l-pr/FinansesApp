namespace FinancesWebApi.Models;

public class Expense
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public float Amount { get; set; }
    public int ExpenseCategoryId { get; set; }
    public DateTime Date { get; set; }
}
