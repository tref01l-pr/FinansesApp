using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancesWebApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string Password { get; set; }
        public int? PhoneNumberId { get; set; } = null;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public DateTime DateOfRegistration { get; set; } = DateTime.Now;
        public int UserSettingsId { get; set; }
        public UserSettings UserSettings { get; set; }
        public UserPhoneNumber UserPhoneNumber { get; set; }
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}