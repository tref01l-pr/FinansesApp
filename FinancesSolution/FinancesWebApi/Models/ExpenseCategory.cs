namespace FinancesWebApi.Models;

public class ExpenseCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float PannedOutlayAmount { get; set; }
    public int ExpenseIconId { get; set; }
    public int ColorId { get; set; }
    public ExpenseIcon ExpenseIcon { get; set; }
    public IconColor IconColor { get; set; }
    private ICollection<ExpenseCategory> ExpenseCategories { get; set; }
}