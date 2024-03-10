using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models.User.UserSettings;

public class CountryPhoneNumber
{
    [Key]
    public string Code { get; set; }
    public string Name { get; set; }
    public string DialCode { get; set; }
    public string Flag { get; set; }
    public string Mask { get; set; }
    public List<UserPhoneNumber> UserPhoneNumbers { get; set; }
}