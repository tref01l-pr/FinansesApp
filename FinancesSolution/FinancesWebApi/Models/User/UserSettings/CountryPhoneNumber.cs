using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models;

public class CountryPhoneNumber
{
    [Key]
    public string Code { get; set; }
    public string Name { get; set; }
    public string DialCode { get; set; }
    public List<UserPhoneNumber> UserPhoneNumbers { get; set; }
}