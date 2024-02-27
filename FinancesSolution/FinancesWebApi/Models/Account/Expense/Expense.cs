using Microsoft.Identity.Client;

namespace FinancesWebApi.Models;

public class Expense
{
    public int ExpenseId { get; set; }
    public int AccountId { get; set; }
    public float Amount { get; set; }
    public string Currency { get; set; }
    public int ExpenseCategoryId { get; set; }
    public DateTime Date { get; set; }
    public Account Account { get; set; }
    public ExpenseCategory ExpenseCategory { get; set; }
}
