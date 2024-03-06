namespace FinancesWebApi.Dto;

public class NumberDto
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required string CountryCode { get; set; }
    public required int Number { get; set; }
}