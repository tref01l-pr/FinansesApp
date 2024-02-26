namespace FinancesWebApi.Models;

public class IncomeIconCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<IncomeIcon> IncomeIcons { get; set; }
}