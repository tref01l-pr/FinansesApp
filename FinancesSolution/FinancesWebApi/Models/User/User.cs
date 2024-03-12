using System.ComponentModel.DataAnnotations;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Models.User
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string? VerificationEmailToken { get; set; } = string.Empty;
        public string? ResetEmailToken { get; set; } = string.Empty;
        public DateTime? VerificationEmailTokenExpires { get; set; } = null;
        public DateTime? ResetEmailTokenExpires { get; set; } = null;
        public DateTime? VerifiedEmailAt { get; set; } = null;
        public bool EmailConfirmed { get; set; } = false;
        public required byte[] PasswordHash { get; set; } = new byte[60];
        public required byte[] PasswordSalt { get; set; } = new byte[60];
        public string? PasswordResetToken { get; set; } = null;
        public DateTime? PasswordResetTokenExpires { get; set; } = null;
        public int? PhoneNumberId { get; set; } = null;
        public string? VerificationPhoneNumberCode { get; set; } = string.Empty;
        public string? ResetPhoneNumberCode { get; set; } = string.Empty;
        public DateTime? VerificationPhoneNumberCodeExpires { get; set; } = null;
        public DateTime? ResetPhoneNumberCodeExpires { get; set; } = null;
        public DateTime? VerifiedPhoneNumberAt { get; set; } = null;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public int NumberOfPasswordAttempts { get; set; } = 5;
        public ICollection<UserRole> UserRoles { get; set; }         
        public UserSettings.UserSettings UserSettings { get; set; }
        public UserPhoneNumber UserPhoneNumber { get; set; }
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}