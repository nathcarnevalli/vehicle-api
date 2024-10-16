﻿using System.ComponentModel.DataAnnotations;
using static ApiVeiculos.Models.ApplicationUser;

namespace ApiVeiculos.DTOs;

public class UserModel
{
    public string? Id { get; set; }
    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(5)]
    public string? UserName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatório")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password é obrigatória")]
    [MinLength(5)]
    [MaxLength(255)]
    public string? PasswordHash { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    public string? CPF { get; set; }

    public EstadoUsuario Estado { get; set; }
}
