namespace FinancesWebApi.Dto;

public class NumberDto
{
    public required int UserId { get; set; }
    public required string CountryCode { get; set; }
    public required string Number { get; set; }
}