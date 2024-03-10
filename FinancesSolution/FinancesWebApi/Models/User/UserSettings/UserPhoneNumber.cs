namespace FinancesWebApi.Models.User.UserSettings;

public class UserPhoneNumber
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required string CountryCode { get; set; }
    public required string Number { get; set; }
    public CountryPhoneNumber CountryPhoneNumber { get; set; }
    public Models.User.User User { get; set; }
}