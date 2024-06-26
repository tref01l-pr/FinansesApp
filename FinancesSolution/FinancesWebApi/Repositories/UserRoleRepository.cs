﻿using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Repositories;

public class UserRoleRepository(DataContext context) : IUserRoleRepository
{
    public Role? GetRoleByName(string name) => context.Roles.FirstOrDefault(r => r.Name == name);
    public async Task<List<UserRole>?> GetRolesByUserIdAsync(int id) => context.UserRoles.Where(ur => ur.UserId == id).ToList();
}