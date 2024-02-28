using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models;

public class IncomeIcon
{
    [Key]
    public int Id { get; set; }
    public int IconCategoryId { get; set; }
    public string Name { get; set; }
    public IconCategory IconCategory { get; set; }
    public ICollection<IncomeCategory> IncomeCategories { get; set; } = new List<IncomeCategory>();
}