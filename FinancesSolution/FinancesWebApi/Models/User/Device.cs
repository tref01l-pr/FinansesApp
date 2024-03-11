using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Models.User;

public class Device
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Ip { get; set; }
    public required string BrowserName { get; set; }
    public required string BrowserVersion { get; set; }
    public required string PlatformName { get; set; }
    public required string PlatformVersion { get; set; }
    public required string PlatformProcessor { get; set; }
    public required string Token { get; set; }
    public DateTime TokenCreated { get; set; } = DateTime.Now;
    public required DateTime TokenExpires { get; set; }
    public User User { get; set; }
}