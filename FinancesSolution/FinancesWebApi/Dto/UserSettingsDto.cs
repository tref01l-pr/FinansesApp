namespace FinancesWebApi.Dto;

public class UserSettingsDto
{
    public int Id { get; set; }
    public string NickName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required DateTime DateOfRegistration { get; set; }
    public string Language { get; set; }
    public int? DefaultAccountId { get; set; }
    public bool NoRounding { get; set; }
    public string Theme { get; set; }
    public string DecimalSeparator { get; set; }
    public string FirstDayOfWeek { get; set; }
    public string Currency { get; set; }
    public string AvatarImage { get; set; }
}