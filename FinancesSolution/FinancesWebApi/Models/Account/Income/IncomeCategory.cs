namespace FinancesWebApi.Models;

public class IncomeCategory
{
    public int IncomeCategoryId { get; set; }
    public string Name { get; set; }
    public float? PannedOutlayAmount { get; set; } = null;
    public int IncomeIconId { get; set; }
    public int ColorId { get; set; }
    public bool IsDefault { get; set; }
    public IncomeIcon IncomeIcon { get; set; }
    public IconColor IconColor { get; set; }
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
}