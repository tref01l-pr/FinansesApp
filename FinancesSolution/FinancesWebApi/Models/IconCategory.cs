namespace FinancesWebApi.Models;

public class IconCategory
{
    public int IncomeIconCategoryId { get; set; }
    public string Name { get; set; }
    public ICollection<IncomeIcon> IncomeIcons { get; set; } = new List<IncomeIcon>();
}