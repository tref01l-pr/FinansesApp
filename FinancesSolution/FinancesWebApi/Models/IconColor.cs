using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models;

public class IconColor
{
    [Key]
    public int Id { get; set; }
    public string Color { get; set; }
    public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();
    public ICollection<IncomeCategory> IncomeCategories { get; set; } = new List<IncomeCategory>();
}