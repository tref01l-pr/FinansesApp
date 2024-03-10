using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models.User;

public class Device
{
    [Key]
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required string Name { get; set; }
    public required string Ip { get; set; }
    public required string Token { get; set; }
    public DateTime TokenCreated { get; set; } = DateTime.Now;
    public required DateTime TokenExpires { get; set; }
    public User User { get; set; }
}