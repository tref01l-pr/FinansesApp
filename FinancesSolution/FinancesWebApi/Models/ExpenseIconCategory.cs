namespace FinancesWebApi.Models;

public class ExpenseIconCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<ExpenseIcon> ExpenseIcons { get; set; }
}