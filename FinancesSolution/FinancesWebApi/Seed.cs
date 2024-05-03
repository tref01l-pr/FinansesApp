using System.Text.Json;
using FinancesWebApi.Data;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi
{
    public class Seed(DataContext context, IMaskConverter maskConverter, IPasswordSecurityService passwordSecurityService)
    {
        public void SeedDataContext()
        {
            if (!context.CountryPhoneNumbers.Any())
            {
                try
                {
                    string json = File.ReadAllText("./Data/CountryCodesWithMasks.json");
                    List<CountryPhoneNumber> countries = JsonSerializer.Deserialize<List<CountryPhoneNumber>>(json)!;

                    context.CountryPhoneNumbers.AddRange(countries);

                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (!context.IconColors.Any())
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

                    new IconColor { Color = "#2e78cf" },
                    new IconColor { Color = "#4895dd" },
                    new IconColor { Color = "#5ea2e1" },
                    new IconColor { Color = "#73b8e1" },
                    new IconColor { Color = "#8ad1fd" },
                    new IconColor { Color = "#a7dcfb" },

                    new IconColor { Color = "#2234d2" },
                    new IconColor { Color = "#3a4de9" },
                    new IconColor { Color = "#5360fe" },
                    new IconColor { Color = "#7b8aff" },
                    new IconColor { Color = "#9ca6ff" },
                    new IconColor { Color = "#bec4ff" },

                    new IconColor { Color = "#ec8208" },
                    new IconColor { Color = "#fb9904" },
                    new IconColor { Color = "#f1b109" },
                    new IconColor { Color = "#e5ba16" },
                    new IconColor { Color = "#f1cb06" },
                    new IconColor { Color = "#f2e149" },

                    new IconColor { Color = "#eb6f19" },
                    new IconColor { Color = "#fa7518" },
                    new IconColor { Color = "#fb8837" },
                    new IconColor { Color = "#fe9850" },
                    new IconColor { Color = "#ffab6f" },
                    new IconColor { Color = "#ffc59d" },

                    new IconColor { Color = "#db4d87" },
                    new IconColor { Color = "#e36094" },
                    new IconColor { Color = "#e274a1" },
                    new IconColor { Color = "#fc86b8" },
                    new IconColor { Color = "#ffa9cc" },
                    new IconColor { Color = "#ffb7d4" },

                    new IconColor { Color = "#e030b9" },
                    new IconColor { Color = "#e93fc3" },
                    new IconColor { Color = "#eb54c8" },
                    new IconColor { Color = "#ec5fcf" },
                    new IconColor { Color = "#ef7ddd" },
                    new IconColor { Color = "#ffa3f0" },

                    new IconColor { Color = "#19bbc6" },
                    new IconColor { Color = "#23cbcb" },
                    new IconColor { Color = "#3dd4df" },
                    new IconColor { Color = "#49e1d2" },
                    new IconColor { Color = "#68e5dd" },
                    new IconColor { Color = "#7bf8e6" },

                    new IconColor { Color = "#0c9384" },
                    new IconColor { Color = "#21b2a1" },
                    new IconColor { Color = "#35cebc" },
                    new IconColor { Color = "#4bdecc" },
                    new IconColor { Color = "#7aebd7" },
                    new IconColor { Color = "#8bf0de" },

                    new IconColor { Color = "#ec494a" },
                    new IconColor { Color = "#f45454" },
                    new IconColor { Color = "#fa5a4e" },
                    new IconColor { Color = "#fd6f63" },
                    new IconColor { Color = "#ff867d" },
                    new IconColor { Color = "#feb3ad" },

                    new IconColor { Color = "#de2020" },
                    new IconColor { Color = "#f63635" },
                    new IconColor { Color = "#ff4855" },
                    new IconColor { Color = "#fe616a" },
                    new IconColor { Color = "#fe616a" },
                    new IconColor { Color = "#ff9ea5" },

                    new IconColor { Color = "#9255ce" },
                    new IconColor { Color = "#9e75d5" },
                    new IconColor { Color = "#a788d6" },
                    new IconColor { Color = "#c2a3f3" },
                    new IconColor { Color = "#d2baf8" },
                    new IconColor { Color = "#dfc8ff" },

                    new IconColor { Color = "#7455cd" },
                    new IconColor { Color = "#7455cd" },
                    new IconColor { Color = "#9d7ef3" },
                    new IconColor { Color = "#b095fe" },
                    new IconColor { Color = "#b8b1ff" },
                    new IconColor { Color = "#d3d0ff" },

                    new IconColor { Color = "#242424" },
                    new IconColor { Color = "#414141" },
                    new IconColor { Color = "#616161" },
                    new IconColor { Color = "#888888" },
                    new IconColor { Color = "#a6a6a6" },
                    new IconColor { Color = "#c9c9c9" },
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

                var roles = new List<Role>
                {
                    new Role() { Name = "user" },
                    new Role() { Name = "admin" },
                    new Role() { Name = "moderator" }
                };


                try
                {
                    context.IconColors.AddRange(iconColors);
                    context.IconCategories.AddRange(iconCategories);
                    context.ExpenseIcons.AddRange(expenseIcons);
                    context.ExpenseCategories.AddRange(expenseCategories);
                    context.IncomeIcons.AddRange(incomeIcons);
                    context.IncomeCategories.AddRange(incomeCategories);
                    context.Roles.AddRange(roles);

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                }
            }


            if (!context.Users.Any())
            {

                var users = new List<User>();
                
                passwordSecurityService.CreatePasswordHash("test1", out byte[] passwordHash, out byte[] passwordSalt);
                
                users.Add(new User
                {
                    UserName = "roman",
                    Email = "example@example.com",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    UserRoles = new List<UserRole>
                    {
                        new UserRole() { Role = context.Roles.First(r => r.Name == "user") },
                        new UserRole() { Role = context.Roles.First(r => r.Name == "admin") }
                    },
                    EmailConfirmed = true,
                    VerifiedEmailAt = DateTime.Now
                });
                
                passwordSecurityService.CreatePasswordHash("test2", out passwordHash, out passwordSalt);
                
                users.Add(new User
                {
                    UserName = "john",
                    Email = "john@example.com",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    UserRoles = new List<UserRole>
                    {
                        new UserRole() { Role = context.Roles.First(r => r.Name == "user") }
                    },
                    EmailConfirmed = true,
                    VerifiedEmailAt = DateTime.Now
                });

                string imagePath = @"C:\Users\prots\Desktop\Everything that was at old Windows\RiderProjects\FinansesApp\FinancesSolution\FinancesWebApi\Data\DefaultImage.jpg";
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string imageBase64 = Convert.ToBase64String(imageBytes);
                    
                foreach (var user in users)
                {
                    var userSettings = new UserSettings
                    {
                        NickName = user.UserName,
                        DateOfRegistration = DateTime.Now,
                        Language = "English",
                        NoRounding = true,
                        Theme = "Light",
                        DecimalSeparator = ".",
                        FirstDayOfWeek = "Monday",
                        Currency = "USD",
                        AvatarImage = imageBase64,
                        User = user
                    };

                    context.UserSettings.Add(userSettings);

                    user.UserSettings = userSettings;

                    var mainAccount = new Account()
                    {
                        Title = "main",
                        Comments = "Main account",
                        Expenses = new List<Expense>(),
                        Incomes = new List<Income>(),
                        User = user
                    };
                    user.Accounts.Add(mainAccount);

                    context.Accounts.Add(mainAccount);
                    context.Users.Add(user);
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                }
            }
        }
    }
}