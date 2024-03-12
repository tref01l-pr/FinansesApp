namespace FinancesWebApi.Models.User.UserSettings;

public class UserPhoneNumber
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CountryCode { get; set; }
    public required string Number { get; set; }
    public CountryPhoneNumber CountryPhoneNumber { get; set; }
    public User User { get; set; }
}