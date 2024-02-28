using FinancesWebApi.Models;
using System;
using System.Collections.Generic;

namespace FinancesWebApi.Data
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedDataContext()
        {
            if (!_context.IconColors.Any())
            {
                var iconColors = new List<IconColor>
                {
                    new IconColor { Color = "#4e8e2e" },
                    new IconColor { Color = "#67ae44" },
                    new IconColor { Color = "#80cf5c" },
                    new IconColor { Color = "#8dd66b" },
                    new IconColor { Color = "#a3dd88" },
                    new IconColor { Color = "#b7e3a4" },

                    new IconColor { Color = "#52664a" },
                    new IconColor { Color = "#6a8260" },
                    new IconColor { Color = "#859d79" },
                    new IconColor { Color = "#9bb58e" },
                    new IconColor { Color = "#bad6ad" },
                    new IconColor { Color = "#d2f1c5" },
                };

                var iconCategories = new List<IconCategory>
                {
                    new IconCategory { Name = "Finances" },
                    new IconCategory { Name = "Transportation" },
                    new IconCategory { Name = "Shopping" },
                    new IconCategory { Name = "Food And Drink" },
                    new IconCategory { Name = "Home" },
                    new IconCategory { Name = "Health" },
                    new IconCategory { Name = "Beauty" },
                    new IconCategory { Name = "Entertainment" },
                    new IconCategory { Name = "Accounts" },
                    new IconCategory { Name = "Workout" },
                    new IconCategory { Name = "Relaxation" },
                    new IconCategory { Name = "Education" },
                    new IconCategory { Name = "Family/Children" },
                    new IconCategory { Name = "Farm" },
                    new IconCategory { Name = "Other" },
                };


                var expenseIcons = new List<ExpenseIcon>
                {
                    new ExpenseIcon { Name = "coins", IconCategory = iconCategories[0] },
                    new ExpenseIcon { Name = "car", IconCategory = iconCategories[1] },
                    new ExpenseIcon { Name = "bicycle", IconCategory = iconCategories[1] },
                    new ExpenseIcon { Name = "car-side", IconCategory = iconCategories[1] },
                    new ExpenseIcon { Name = "plane", IconCategory = iconCategories[1] },
                };

                var expenseCategories = new List<ExpenseCategory>
                {
                    new ExpenseCategory
                        { Name = "Health", IsDefault = true, ExpenseIcon = expenseIcons[0], IconColor = iconColors[0] },
                    new ExpenseCategory
                        { Name = "Home", IsDefault = true, ExpenseIcon = expenseIcons[1], IconColor = iconColors[1] }
                };


                var incomeIcons = new List<IncomeIcon>
                {
                    new IncomeIcon { Name = "coins", IconCategory = iconCategories[2] },
                    new IncomeIcon { Name = "car", IconCategory = iconCategories[4] },
                    new IncomeIcon { Name = "bicycle", IconCategory = iconCategories[3] },
                    new IncomeIcon { Name = "car-side", IconCategory = iconCategories[6] },
                    new IncomeIcon { Name = "plane", IconCategory = iconCategories[2] },
                };

                var incomeCategories = new List<IncomeCategory>
                {
                    new IncomeCategory
                        { Name = "Health", IsDefault = true, IncomeIcon = incomeIcons[0], IconColor = iconColors[0] },
                    new IncomeCategory
                        { Name = "Home", IsDefault = true, IncomeIcon = incomeIcons[1], IconColor = iconColors[1] }
                };


                try
                {
                    _context.IconColors.AddRange(iconColors);
                    _context.IconCategories.AddRange(iconCategories);
                    _context.ExpenseIcons.AddRange(expenseIcons);
                    _context.ExpenseCategories.AddRange(expenseCategories);
                    _context.IncomeIcons.AddRange(incomeIcons);
                    _context.IncomeCategories.AddRange(incomeCategories);

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                }
            }


            if (!_context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        UserName = "roman",
                        NormalizedUserName = "ROMAN",
                        Email = "example@example.com", 
                        NormalizedEmail = "EXAMPLE@EXAMPLE.COM",
                        Password = BCrypt.Net.BCrypt.HashPassword("test1"),
                        DateOfRegistration = DateTime.Now,
                        Accounts = new List<Account>(),
                        UserSettings = new UserSettings
                        {
                            Language = "English",
                            NoRounding = true,
                            Theme = "Light",
                            DecimalSeparator = ".",
                            FirstDayOfWeek = "Monday",
                            Currency = "USD"
                        }
                    },
                    new User
                    {
                        UserName = "john",
                        NormalizedUserName = "JOHN",
                        Email = "john@example.com",
                        NormalizedEmail = "JOHN@EXAMPLE.COM",
                        Password = BCrypt.Net.BCrypt.HashPassword("test2"),
                        DateOfRegistration = DateTime.Now,
                        Accounts = new List<Account>(),
                        UserSettings = new UserSettings()
                        {
                            Language = "English",
                            NoRounding = true,
                            Theme = "Light",
                            DecimalSeparator = ".",
                            FirstDayOfWeek = "Monday",
                            Currency = "USD"
                        }
                    },
                };

                foreach (var user in users)
                {
                    var mainAccount = new Account()
                    {
                        Title = "main",
                        Comments = "Main account",
                        Expenses = new List<Expense>(),
                        Incomes = new List<Income>(),
                        User = user
                    };
                    user.Accounts.Add(mainAccount);

                    _context.Users.Add(user);
                }

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                }
            }
        }
    }
}