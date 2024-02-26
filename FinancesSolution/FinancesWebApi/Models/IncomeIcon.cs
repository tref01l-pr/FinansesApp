namespace FinancesWebApi.Models;

public class IncomeIcon
{
    public int Id { get; set; }
    public int IncomeIconId { get; set; }
    public string Name { get; set; }
    public IncomeIconCategory IncomeIconCategory { get; set; }
    public ICollection<IncomeCategory> IncomeCategories { get; set; }
}