namespace FinancesWebApi.Models;

public class IconColor
{
    public int Id { get; set; }
    public string Color { get; set; }
    public ICollection<ExpenseCategory> ExpenseCategories { get; set; }
    public ICollection<IncomeCategory> IncomeCategories { get; set; }
}