namespace FinancesWebApi.Models;

public class Income
{
    public int IncomeId { get; set; }
    public int AccountId { get; set; }
    public float Amount { get; set; }
    public string Currency { get; set; }
    public int IncomeCategoryId { get; set; }
    public DateTime Date { get; set; }
    public  Account Account { get; set; }
    public IncomeCategory IncomeCategory { get; set; }
}