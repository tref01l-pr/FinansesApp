using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models;

public class ExpenseCategory
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public float? PannedOutlayAmount { get; set; } = null;
    public int ExpenseIconId { get; set; }
    public int ColorId { get; set; }
    public bool IsDefault { get; set; }
    public ExpenseIcon ExpenseIcon { get; set; }
    public IconColor IconColor { get; set; }
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}