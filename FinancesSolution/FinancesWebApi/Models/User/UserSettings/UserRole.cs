namespace FinancesWebApi.Models.User.UserSettings;

public class UserRole
{
    public int UserId { get; set; }
    public Models.User.User User { get; set; }

    public int RoleId { get; set; }
    public /*virtual */Role Role { get; set; }
}