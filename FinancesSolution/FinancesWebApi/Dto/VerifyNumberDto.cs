using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Dto;

public class VerifyNumberDto
{
    [Required]
    public string Code { get; set; }
    [Required]
    public string CountryCode { get; set; }
    [Required]
    public string Number { get; set; }
}