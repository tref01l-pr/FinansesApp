﻿using System.ComponentModel.DataAnnotations;

namespace FinancesWebApi.Dto;

public class RegisterDto
{
    [Required, MinLength(6)] 
    public string UserName { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    [Required, Compare("Password")]
    public string PasswordConfirm { get; set; } = string.Empty;
}