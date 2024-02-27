namespace FinancesWebApi.Models;

public class ExpenseIcon
{
    public int ExpenseIconId { get; set; }
    public int IconCategoryId { get; set; }
    public string Name { get; set; }
    public IconCategory IconCategory { get; set; }
    public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();
}