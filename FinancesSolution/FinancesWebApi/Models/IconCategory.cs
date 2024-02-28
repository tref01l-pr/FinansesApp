using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models;

public class IconCategory
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<IncomeIcon> IncomeIcons { get; set; } = new List<IncomeIcon>();
}