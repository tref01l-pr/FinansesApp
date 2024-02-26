namespace FinancesWebApi.Models;

public class ExpenseIcon
{
    public int Id { get; set; }
    public int ExpenseIconCategoryId { get; set; }
    public string Name { get; set; }
    public ExpenseIconCategory ExpenseIconCategory { get; set; }
    public ICollection<ExpenseCategory> ExpenseCategories { get; set; }
}