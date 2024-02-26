namespace FinancesWebApi.Models;

public class IncomeCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float PannedOutlayAmount { get; set; }
    public int IncomeIconId { get; set; }
    public int ColorId { get; set; }
    public IncomeIcon IncomeIcon { get; set; }
    public IconColor IconColor { get; set; }
    private ICollection<IncomeCategory> IncomeCategories { get; set; }
}