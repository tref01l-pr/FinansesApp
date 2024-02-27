namespace FinancesWebApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string Password { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public DateTime DateOfRegistration { get; set; } = DateTime.Now;
        public int UserSettingsId { get; set; }
        public UserSettings UserSettings { get; set; }
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Income> Incomes { get; set; } = new List<Income>();
    }
}