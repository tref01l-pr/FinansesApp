using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models;

public class IncomeCategory
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public float? PannedOutlayAmount { get; set; } = null;
    public int IncomeIconId { get; set; }
    public int ColorId { get; set; }
    public bool IsDefault { get; set; } = false;
    public IncomeIcon IncomeIcon { get; set; }
    public IconColor IconColor { get; set; }
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
}