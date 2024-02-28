using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models;

public class UserSettings
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Language { get; set; } = "English";
    public int? DefaultAccountId { get; set; } = null;
    public bool NoRounding { get; set; } = true;
    public string Theme { get; set; } = "Light";
    public string DecimalSeparator { get; set; } = ".";
    public string FirstDayOfWeek { get; set; } = "Monday";
    public string Currency { get; set; } = "USD";
    public Account DefaultAccount { get; set; }
    public User User { get; set; }
}