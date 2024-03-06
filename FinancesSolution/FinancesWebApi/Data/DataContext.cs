using Microsoft.EntityFrameworkCore;
using FinancesWebApi.Models;

namespace FinancesWebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<ExpenseIcon> ExpenseIcons { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<IncomeCategory> IncomeCategories { get; set; }
        public DbSet<IncomeIcon> IncomeIcons { get; set; }
        public DbSet<IconCategory> IconCategories { get; set; }
        public DbSet<IconColor> IconColors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserPhoneNumber?> UserPhoneNumbers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        
        public DbSet<Role> Roles { get; set; }
        public DbSet<CountryPhoneNumber> CountryPhoneNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .ValueGeneratedNever();
            
            /*modelBuilder.Entity<User>()
                .Property(u => u.UserSettingsId)
                .ValueGeneratedNever();*/
            
            modelBuilder.Entity<UserSettings>()
                .Property(u => u.DateOfRegistration)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Account>()
                .HasMany(a => a.Expenses)
                .WithOne(e => e.Account)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Incomes)
                .WithOne(i => i.Account)
                .HasForeignKey(i => i.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserSettings)
                .WithOne(us => us.User)
                .HasForeignKey<UserSettings>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExpenseCategory>()
                .HasMany(ec => ec.Expenses)
                .WithOne(e => e.ExpenseCategory)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(ec => new { ec.Id, ec.IsDefault });

            modelBuilder.Entity<IncomeCategory>()
                .HasMany(ic => ic.Incomes)
                .WithOne(i => i.IncomeCategory)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(ic => new { ic.Id, ic.IsDefault });

            modelBuilder.Entity<CountryPhoneNumber>()
                .HasMany(cpn => cpn.UserPhoneNumbers)
                .WithOne(upn => upn.CountryPhoneNumber)
                .HasForeignKey(upn => upn.CountryCode);
        }
    }
}
