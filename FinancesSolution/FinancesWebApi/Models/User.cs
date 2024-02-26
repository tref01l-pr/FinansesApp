﻿namespace FinancesWebApi.Models;

public class User
{
    public int Id { get; set; }
    public string NickName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public ICollection<Account> Accounts { get; set; }
}