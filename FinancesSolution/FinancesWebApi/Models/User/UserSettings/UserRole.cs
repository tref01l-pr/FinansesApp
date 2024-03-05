namespace FinancesWebApi.Models;

public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int RoleId { get; set; }
    public /*virtual */Role Role { get; set; }
}