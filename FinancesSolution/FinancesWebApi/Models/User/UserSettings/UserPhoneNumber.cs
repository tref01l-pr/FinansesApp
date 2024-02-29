namespace FinancesWebApi.Models;

public class UserPhoneNumber
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CountryId { get; set; }
    public string Number { get; set; }
    public CountryPhoneNumber CountryPhoneNumber { get; set; }
    public User User { get; set; }
}